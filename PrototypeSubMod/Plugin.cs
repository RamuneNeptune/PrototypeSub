using BepInEx;
using BepInEx.Logging;
using EpicStructureLoader;
using HarmonyLib;
using ModStructureFormat;
using Nautilus.Handlers;
using Newtonsoft.Json;
using PrototypeSubMod.Compatibility;
using PrototypeSubMod.PowerSystem;
using PrototypeSubMod.Prefabs;
using PrototypeSubMod.Prefabs.UpgradePlatforms;
using PrototypeSubMod.SaveData;
using PrototypeSubMod.Upgrades;
using PrototypeSubMod.Utility;
using SubLibrary.Audio;
using System.Collections;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace PrototypeSubMod
{
    [BepInPlugin(GUID, pluginName, versionString)]
    [BepInDependency("com.snmodding.nautilus")]
    [BepInDependency("com.indigocoder.sublibrary")]
    [BepInDependency("com.lee23.epicstructureloader", "1.0.1")]
    [BepInDependency("ArchitectsLibrary", BepInDependency.DependencyFlags.SoftDependency)]
    public class Plugin : BaseUnityPlugin
    {
        private const string GUID = "com.teamproto.prototypesub";
        private const string pluginName = "Prototype Sub";
        private const string versionString = "0.0.5.0";

        public new static ManualLogSource Logger { get; private set; }

        private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

        public static string AssetsFolderPath { get; } = Path.Combine(Path.GetDirectoryName(Assembly.Location), "Assets");
        public static string RecipesFolderPath { get; } = Path.Combine(Path.GetDirectoryName(Assembly.Location), "Recipes");

        public static AssetBundle AssetBundle { get; } = AssetBundle.LoadFromFile(Path.Combine(AssetsFolderPath, "prototypeassets"));

        public static EquipmentType PrototypePowerType { get; } = EnumHandler.AddEntry<EquipmentType>("PrototypePowerType");
        public static EquipmentType LightBeaconEquipmentType { get; } = EnumHandler.AddEntry<EquipmentType>("LightBeaconType");

        public static TechGroup PrototypeGroup { get; } = EnumHandler.AddEntry<TechGroup>("PrototypeSub").WithPdaInfo(null);
        public static TechCategory PrototypeCategory { get; } = EnumHandler.AddEntry<TechCategory>("PrototypeSub").RegisterToTechGroup(PrototypeGroup)
            .WithPdaInfo(null);

        public static TechCategory ProtoModuleCategory { get; } = EnumHandler.AddEntry<TechCategory>("ProtoModules").RegisterToTechGroup(PrototypeGroup)
            .WithPdaInfo(null);

        internal static BatterySaveData BatterySaveData = SaveDataHandler.RegisterSaveDataCache<BatterySaveData>();
        internal static GameObject welderPrefab;

        private static bool Initialized;

        private void Awake()
        {
            if (Initialized) return;

            // Set project-scoped logger instance
            Logger = base.Logger;

            LanguageHandler.RegisterLocalizationFolder();
            //PrototypeAudio.RegisterAudio(AssetBundle);
            SubAudioLoader.LoadAllAudio(AssetBundle);

            RegisterEncyEntries();
            RegisterStructures();
            RegisterPrefabs();
            RegisterStoryGoals();
            InitializeSlotMapping();

            LoadEasyPrefabs.LoadPrefabs(AssetBundle);
            ROTACompatManager.AddCompatiblePowerSources();

            // Register harmony patches, if there are any
            Harmony.CreateAndPatchAll(Assembly, $"{GUID}");
            Logger.LogInfo($"Plugin {GUID} is loaded!");
        }

        private IEnumerator Start()
        {
            if (Initialized) yield break;

            StartCoroutine(AddBatteryComponents());
            Initialized = true;

            yield return new WaitUntil(() => CraftData.cacheInitialized && CraftTree.initialized);
            yield return new WaitForEndOfFrame();

            var task = CraftData.GetPrefabForTechTypeAsync(TechType.Welder);
            yield return task;

            welderPrefab = task.GetResult();

            UpgradeUninstallationPrefabManager.RegisterUninstallationPrefabs(AssetBundle);
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

        private void RegisterPrefabs()
        {
            PrecursorIngot_Craftable.Register();
            IonPrism_Craftable.Register();

            Prototype_Craftable.Register();
            ProtoBuildTerminal_World.Register();
            DeployableLight_Craftable.Register();
            ProtoRepairBot_Spawned.Register();
            CrystalMatrix_Craftable.Register();

            ProtoPlaque_World.Register();
            ProtoLogo_World.Register();
            DamagedProtoLogo_World.Register();
            TeleporterTerminal_World.Register();
        }

        private void RegisterEncyEntries()
        {
            #region Precursor Ingot
            LanguageHandler.SetLanguageLine("DownloadedData/Precursor/Scan", "ProtoPrecursorIngotEncyLine");

            string title = Language.main.Get("ProtoPrecursorIngotEncyTitle");
            string description = Language.main.Get("ProtoPrecursorIngotEncyBody");

            PDAHandler.AddEncyclopediaEntry("ProtoPrecursorIngot", "DownloadedData/Precursor/Scan", title, description, unlockSound: PDAHandler.UnlockBasic);
            #endregion

            #region Interceptor Terminal
            LanguageHandler.SetLanguageLine("DownloadedData/Precursor/Terminal", "ProtoTeleporterEncyEntry");

            string protoTeleporterText = Language.main.Get("ProtoTeleporterEncyEntry");
            string protoTeleporterTextBody = Language.main.Get("ProtoTeleporterEncyEntry_Body");
            Texture2D image = AssetBundle.LoadAsset<Texture2D>("ProtoTeleporter_Corrupted");

            PDAHandler.AddEncyclopediaEntry("InterceptorTestEncy", "DownloadedData/Precursor/Scan", protoTeleporterText, protoTeleporterTextBody, image, unlockSound: PDAHandler.UnlockBasic);
            #endregion
        }

        private void RegisterStoryGoals()
        {
            #region Precursor Ingot

            StoryGoalHandler.RegisterCompoundGoal("Ency_ProtoPrecursorIngot", Story.GoalType.Encyclopedia, 12f, "Goal_BiomePrecursorGunUpper");

            StoryGoalHandler.RegisterCustomEvent("Ency_ProtoPrecursorIngot", () =>
            {
                KnownTech.Add(PrecursorIngot_Craftable.prefabInfo.TechType);
                PDAEncyclopedia.Add("ProtoPrecursorIngot", true);
            });

            #endregion

            #region Interceptor Unlock

            StoryGoalHandler.RegisterCustomEvent("OnInterceptorTestDataDownloaded", () =>
            {
                PDALog.Add("OnInterceptorTestDataDownloaded");
            });

            StoryGoalHandler.RegisterCompoundGoal("InterceptorTestEncy", Story.GoalType.Encyclopedia, 15f, new[] { "OnInterceptorTestDataDownloaded" });
            StoryGoalHandler.RegisterCustomEvent("InterceptorTestEncy", () =>
            {
                PDAEncyclopedia.Add("InterceptorTestEncy", true);
            });

            #endregion
        }

        private void RegisterStructures()
        {
            var interceptorStructure = AssetBundle.LoadAsset<TextAsset>("InterceptorUnlockStructure");
            var structure = JsonConvert.DeserializeObject<Structure>(interceptorStructure.text);

            int entityCount = 0;
            StructureLoading.RegisterStructure(structure, ref entityCount);
        }
    }
}