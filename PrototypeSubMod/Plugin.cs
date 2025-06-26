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
using PrototypeSubMod.Utility;
using SubLibrary.Audio;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Threading;
using PrototypeSubMod.MiscMonobehaviors;
using PrototypeSubMod.Pathfinding.SaveSystem;
using UnityEngine;
using UWE;

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
        private const string versionString = "0.27.0";

        public new static ManualLogSource Logger { get; private set; }

        internal static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

        public static string AssetsFolderPath { get; } = Path.Combine(Path.GetDirectoryName(Assembly.Location), "Assets");
        public static string RecipesFolderPath { get; } = Path.Combine(Path.GetDirectoryName(Assembly.Location), "Recipes");

        public static AssetBundle AssetBundle { get; } = AssetBundle.LoadFromFile(Path.Combine(AssetsFolderPath, "prototypeassets"));
        public static AssetBundle AudioBundle { get; } = AssetBundle.LoadFromFile(Path.Combine(AssetsFolderPath, "prototypeaudio"));
        public static AssetBundle ScenesAssetBundle { get; } = AssetBundle.LoadFromFile(Path.Combine(AssetsFolderPath, "prototypescenes"));
        
        public static EquipmentType PrototypePowerType { get; } = EnumHandler.AddEntry<EquipmentType>("PrototypePowerType");
        public static EquipmentType LightBeaconEquipmentType { get; } = EnumHandler.AddEntry<EquipmentType>("LightBeaconType");

        public static TechGroup PrototypeGroup { get; } = EnumHandler.AddEntry<TechGroup>("PrototypeSub").WithPdaInfo(null);
        public static TechCategory PrototypeCategory { get; } = EnumHandler.AddEntry<TechCategory>("PrototypeSub").RegisterToTechGroup(PrototypeGroup)
            .WithPdaInfo(null);

        public static TechCategory ProtoModuleCategory { get; } = EnumHandler.AddEntry<TechCategory>("ProtoModules").RegisterToTechGroup(PrototypeGroup)
            .WithPdaInfo(null);
        
        public static TechGroup ProtoFabricatorGroup { get; } = EnumHandler.AddEntry<TechGroup>("ProtoFabricator").WithPdaInfo(null);
        public static TechCategory ProtoFabricatorCatgeory { get; } = EnumHandler.AddEntry<TechCategory>("ProtoFabricator").RegisterToTechGroup(ProtoFabricatorGroup)
            .WithPdaInfo(null);
        
        internal static ProtoGlobalSaveData GlobalSaveData = SaveDataHandler.RegisterSaveDataCache<ProtoGlobalSaveData>();
        internal static GameObject welderPrefab;

        internal static Sprite PrototypeSaveIcon = AssetBundle.LoadAsset<Sprite>("ProtoSaveIcon");
        internal static PingType PrototypePingType = EnumHandler.AddEntry<PingType>("PrototypeSub")
            .WithIcon(AssetBundle.LoadAsset<Sprite>("Proto_HUD_Marker"));

        internal const string DEFENSE_CHAMBER_BIOME_NAME = "protodefensefacility";
        internal const string ENGINE_FACILITY_BIOME_NAME = "protoenginefacility";
        internal static readonly Vector3 STORY_END_POS = new Vector3(-1333, -900, -3014);
        internal static readonly Vector3 DEFENSE_PING_POS = new Vector3(700, -489, -1456);
        internal static TechType StoryEndPingTechType;
        internal static GridSaveData pathfindingGridSaveData;
        internal static event Action<GridSaveData> onLoadGridSaveData;

        private static bool Initialized;
        private static Harmony harmony = new Harmony(GUID);

        public static WaitScreen.ManualWaitItem prefabLoadWaitItem;
        public static bool easyPrefabsLoaded;

        private void Awake()
        {
            if (Initialized) return;

            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            // Set project-scoped logger instance
            Logger = base.Logger;
            
            // Register harmony patches, if there are any
            harmony.PatchAll(Assembly);

            var audioSW = new System.Diagnostics.Stopwatch();
            audioSW.Start();
            foreach (var asset in AudioBundle.LoadAllAssets())
            {
                if (asset is not CustomFMODAsset fmodAsset) continue;
                
                SubAudioLoader.RegisterAssetAudio(fmodAsset);
            }
            audioSW.Stop();
            Logger.LogInfo($"Audio registered in {audioSW.ElapsedMilliseconds}ms");

            var databaseSW = new System.Diagnostics.Stopwatch();
            databaseSW.Start();
            ProtoMatDatabase.Initalize();
            databaseSW.Stop();
            Logger.LogInfo($"Material database registered in {databaseSW.ElapsedMilliseconds}ms");
            
            LanguageHandler.RegisterLocalizationFolder();
            PrefabRegisterer.Register();
            LoadEasyPrefabs.LoadPrefabs(AssetBundle, EncyEntryRegisterer.Register, ClearWaitStage);
            StructureRegisterer.Register();
            StoryGoalsRegisterer.Register();
            BiomeRegisterer.Register();
            LootRegister.Register();
            CommandRegisterer.Register();
            PDAMessageRegisterer.Register();
            
            var voicelineSW = new System.Diagnostics.Stopwatch();
            voicelineSW.Start();
            VoicelineRegisterer.UpdateVoicelines();
            voicelineSW.Stop();
            Logger.LogInfo($"Voiceline variations registered in {sw.ElapsedMilliseconds}ms");
            RegisterDependantPatches();
            InitializeSlotMapping();
            LoadPathfindingGrid();
            
            var miscSW = new System.Diagnostics.Stopwatch();
            miscSW.Start();
            ConsoleCommandsHandler.RegisterConsoleCommands(typeof(PrototypeCommands));
            ROTACompatManager.AddCompatiblePowerSources();
            WeatherCompatManager.Initialize();
            SetupSaveStateReferences.SetupReferences(Assembly);
            miscSW.Stop();
            Logger.LogInfo($"Miscellaneous items registered in {miscSW.ElapsedMilliseconds}ms");
            
            CoroutineHost.StartCoroutine(Initialize());
            CoroutineHost.StartCoroutine(MakeSeaTreaderBlockersPassthrough());

            // This is only to force the asset bundle to load
            var empty = ScenesAssetBundle.name;

            sw.Stop();
            Logger.LogInfo($"Plugin {GUID} is loaded in {sw.ElapsedMilliseconds} ms!");
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

        private void ClearWaitStage()
        {
            easyPrefabsLoaded = true;
            
            if (prefabLoadWaitItem == null) return;

            WaitScreen.Remove(prefabLoadWaitItem);
        }

        private void InitializeSlotMapping()
        {
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            
            foreach (string name in PrototypePowerSystem.SLOT_NAMES)
            {
                Equipment.slotMapping.Add(name, PrototypePowerType);
            }

            Equipment.slotMapping.Add(ProtoPowerAbilitySystem.SlotName, PrototypePowerType);
            sw.Stop();
            Logger.LogInfo($"Slot mapping registered in {sw.ElapsedMilliseconds}ms");
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

        private IEnumerator MakeSeaTreaderBlockersPassthrough()
        {
            var task = PrefabDatabase.GetPrefabAsync("626f6739-acb0-4dfc-bbab-9b627767403c");
            yield return task;

            task.TryGetPrefab(out var prefab);
            prefab.EnsureComponent<DontCollideWithPlayer>();
        }

        private void RegisterDependantPatches()
        {
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            if (Chainloader.PluginInfos.ContainsKey("com.danithedani.deepercreatures"))
            {
                var structureTranspiler = AccessTools.Method(typeof(StructureLoading_Patches), nameof(StructureLoading_Patches.RegisterStructure_Transpiler));
                var originalMethod = AccessTools.Method(typeof(StructureLoading), nameof(StructureLoading.RegisterStructure));
                harmony.Patch(originalMethod, transpiler: new HarmonyMethod(structureTranspiler));
            }

            sw.Stop();
            Logger.LogInfo($"Dependant patches registered in {sw.ElapsedMilliseconds}ms");
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