using System;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using EpicStructureLoader;
using HarmonyLib;
using Nautilus.Handlers;
using PrototypeSubMod.Commands;
using PrototypeSubMod.Compatibility;
using PrototypeSubMod.Patches;
using PrototypeSubMod.PowerSystem;
using PrototypeSubMod.Registration;
using PrototypeSubMod.SaveData;
using PrototypeSubMod.Upgrades;
using PrototypeSubMod.Utility;
using SubLibrary.Audio;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Threading;
using PrototypeSubMod.Pathfinding.SaveSystem;
using UnityEngine;

namespace PrototypeSubMod
{
    [BepInPlugin(GUID, pluginName, versionString)]
    [BepInDependency("com.snmodding.nautilus")]
    [BepInDependency("com.indigocoder.sublibrary")]
    [BepInDependency("com.lee23.epicstructureloader", "1.0.1")]
    [BepInDependency("com.alembic.package")]
    [BepInDependency("ArchitectsLibrary", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.lee23.theredplague", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.danithedani.deepercreatures", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.lee23.epicweather", BepInDependency.DependencyFlags.SoftDependency)]
    public class Plugin : BaseUnityPlugin
    {
        private const string GUID = "com.prototech.prototypesub";
        private const string pluginName = "Prototype Sub";
        private const string versionString = "0.19.0";

        public new static ManualLogSource Logger { get; private set; }

        internal static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

        public static string AssetsFolderPath { get; } = Path.Combine(Path.GetDirectoryName(Assembly.Location), "Assets");
        public static string RecipesFolderPath { get; } = Path.Combine(Path.GetDirectoryName(Assembly.Location), "Recipes");

        public static AssetBundle AssetBundle { get; } = AssetBundle.LoadFromFile(Path.Combine(AssetsFolderPath, "prototypeassets"));
        public static AssetBundle ScenesAssetBundle { get; } = AssetBundle.LoadFromFile(Path.Combine(AssetsFolderPath, "prototypescenes"));
        
        public static EquipmentType PrototypePowerType { get; } = EnumHandler.AddEntry<EquipmentType>("PrototypePowerType");
        public static EquipmentType LightBeaconEquipmentType { get; } = EnumHandler.AddEntry<EquipmentType>("LightBeaconType");

        public static TechGroup PrototypeGroup { get; } = EnumHandler.AddEntry<TechGroup>("PrototypeSub").WithPdaInfo(null);
        public static TechCategory PrototypeCategory { get; } = EnumHandler.AddEntry<TechCategory>("PrototypeSub").RegisterToTechGroup(PrototypeGroup)
            .WithPdaInfo(null);

        public static TechCategory ProtoModuleCategory { get; } = EnumHandler.AddEntry<TechCategory>("ProtoModules").RegisterToTechGroup(PrototypeGroup)
            .WithPdaInfo(null);

        public static PingType DefenseFacilityPingType = EnumHandler.AddEntry<PingType>("DefenseFacility")
            .WithIcon(AssetBundle.LoadAsset<Sprite>("DefenseFacilityLogo"));
        
        internal static ProtoGlobalSaveData GlobalSaveData = SaveDataHandler.RegisterSaveDataCache<ProtoGlobalSaveData>();
        internal static GameObject welderPrefab;

        internal static Sprite PrototypeSaveIcon = AssetBundle.LoadAsset<Sprite>("ProtoSaveIcon");
        internal static PingType PrototypePingType = EnumHandler.AddEntry<PingType>("PrototypeSub")
            .WithIcon(AssetBundle.LoadAsset<Sprite>("Proto_HUD_Marker"));

        internal const string DEFENSE_CHAMBER_BIOME_NAME = "protodefensefacility";
        internal const string ENGINE_FACILITY_BIOME_NAME = "protoenginefacility";
        internal static readonly Vector3 STORY_END_POS = new Vector3(-1333, -900, -3014);
        internal static readonly Vector3 DEFENSE_PING_POS = new Vector3(701, -366, -1359);
        internal static TechType DefenseFacilityPingTechType;
        internal static TechType StoryEndPingTechType;
        internal static GridSaveData pathfindingGridSaveData;
        internal static event Action<GridSaveData> onLoadGridSaveData;

        private static bool Initialized;
        private static Harmony harmony = new Harmony(GUID);

        private void Awake()
        {
            if (Initialized) return;

            // Set project-scoped logger instance
            Logger = base.Logger;

            LanguageHandler.RegisterLocalizationFolder();
            SubAudioLoader.LoadAllAudio(AssetBundle);

            PrefabRegisterer.Register();
            EncyEntryRegisterer.Register();
            StructureRegisterer.Register();
            StoryGoalsRegisterer.Register();
            BiomeRegisterer.Register();
            LootRegister.Register();
            CommandRegisterer.Register();
            PDAMessageRegisterer.Register();
            VoicelineRegisterer.UpdateVoicelines();
            RegisterDependantPatches();
            InitializeSlotMapping();
            LoadPathfindingGrid();
            
            ConsoleCommandsHandler.RegisterConsoleCommands(typeof(PrototypeCommands));
            LoadEasyPrefabs.LoadPrefabs(AssetBundle);
            ROTACompatManager.AddCompatiblePowerSources();
            WeatherCompatManager.Initialize();
            SetupSaveStateReferences.SetupReferences(Assembly);
            UpgradeUninstallationPrefabManager.RegisterUninstallationPrefabs(AssetBundle);
            
            UWE.CoroutineHost.StartCoroutine(Initialize());

            // This is only to force the asset bundle to load
            var empty = ScenesAssetBundle.name;
            
            // Register harmony patches, if there are any
            harmony.PatchAll(Assembly);
            Logger.LogInfo($"Plugin {GUID} is loaded!");
        }

        private IEnumerator Initialize()
        {
            if (Initialized) yield break;

            yield return AddBatteryComponents();
            Initialized = true;

            yield return new WaitUntil(() => CraftData.cacheInitialized && CraftTree.initialized);
            yield return new WaitForEndOfFrame();

            var task = CraftData.GetPrefabForTechTypeAsync(TechType.Welder);
            yield return task;

            welderPrefab = task.GetResult();
        }

        private void InitializeSlotMapping()
        {
            foreach (string name in PrototypePowerSystem.SLOT_NAMES)
            {
                Equipment.slotMapping.Add(name, PrototypePowerType);
            }

            Equipment.slotMapping.Add(ProtoPowerAbilitySystem.SlotName, PrototypePowerType);
        }

        private IEnumerator AddBatteryComponents()
        {
            foreach (var kvp in PrototypePowerSystem.AllowedPowerSources)
            {
                CoroutineTask<GameObject> prefabTask = CraftData.GetPrefabForTechTypeAsync(kvp.Key);
                yield return prefabTask;

                GameObject prefab = prefabTask.result.Get();
                prefab.AddComponent<PrototypePowerBattery>();
            }
        }

        private void RegisterDependantPatches()
        {
            if (Chainloader.PluginInfos.ContainsKey("com.danithedani.deepercreatures"))
            {
                var structureTranspiler = AccessTools.Method(typeof(StructureLoading_Patches), nameof(StructureLoading_Patches.RegisterStructure_Transpiler));
                var originalMethod = AccessTools.Method(typeof(StructureLoading), nameof(StructureLoading.RegisterStructure));
                harmony.Patch(originalMethod, transpiler: new HarmonyMethod(structureTranspiler));
            }
        }

        private void LoadPathfindingGrid()
        {
            byte[] bytes = AssetBundle.LoadAsset<TextAsset>("SaveGrid.grid").bytes;
            ThreadStart threadStart = () => DeserializeGridData(bytes, saveData =>
            {
                pathfindingGridSaveData = saveData;
                onLoadGridSaveData?.Invoke(saveData);
            });

            var gridLoadThread = new Thread(threadStart);
            gridLoadThread.Start();
        }

        private void DeserializeGridData(byte[] bytes, Action<GridSaveData> callback)
        {
            var data = SaveManager.DeserializeObject<GridSaveData>(bytes);
            callback?.Invoke(data);
        }
    }
}