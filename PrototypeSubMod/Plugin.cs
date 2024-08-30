using BepInEx;
using BepInEx.Logging;
using EpicStructureLoader;
using HarmonyLib;
using ModStructureFormat;
using Nautilus.Handlers;
using Newtonsoft.Json;
using PrototypeSubMod.PowerSystem;
using PrototypeSubMod.Prefabs;
using PrototypeSubMod.Prefabs.UpgradePlatforms;
using PrototypeSubMod.SaveData;
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
    public class Plugin : BaseUnityPlugin
    {
        private const string GUID = "com.teamproto.prototypesub";
        private const string pluginName = "Prototype Sub";
        private const string versionString = "0.0.3";

        public new static ManualLogSource Logger { get; private set; }

        private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

        public static string AssetsFolderPath { get; } = Path.Combine(Path.GetDirectoryName(Assembly.Location), "Assets");
        public static string RecipesFolderPath { get; } = Path.Combine(Path.GetDirectoryName(Assembly.Location), "Recipes");

        public static AssetBundle AssetBundle { get; } = AssetBundle.LoadFromFile(Path.Combine(AssetsFolderPath, "prototypeassets"));

        public static EquipmentType PrototypePowerType { get; } = EnumHandler.AddEntry<EquipmentType>("PrototypePowerType");
        public static EquipmentType LightBeaconEquipmentType { get; } = EnumHandler.AddEntry<EquipmentType>("LightBeaconType");

        internal static BatterySaveData BatterySaveData = SaveDataHandler.RegisterSaveDataCache<BatterySaveData>();

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

            // Register harmony patches, if there are any
            Harmony.CreateAndPatchAll(Assembly, $"{GUID}");
            Logger.LogInfo($"Plugin {GUID} is loaded!");
        }

        private void Start()
        {
            if (Initialized) return;

            StartCoroutine(AddBatteryComponents());
            Initialized = true;
        }

        private void InitializeSlotMapping()
        {
            foreach (string name in PrototypePowerSystem.SLOT_NAMES)
            {
                Equipment.slotMapping.Add(name, PrototypePowerType);
            }
        }

        private IEnumerator AddBatteryComponents()
        {
            foreach (var kvp in PrototypePowerSystem.AllowedPowerSources)
            {
                CoroutineTask<GameObject> prefabTask = CraftData.GetPrefabForTechTypeAsync(kvp.Key);
                yield return prefabTask;

                GameObject prefab = prefabTask.result.Get();
                var battery = prefab.AddComponent<PrototypePowerBattery>();
            }
        }

        private void RegisterPrefabs()
        {
            PrecursorIngot_Craftable.Register();
            Prototype_Craftable.Register();
            ProtoBuildTerminal_World.Register();
            DeployableLight_Craftable.Register();

            ProtoPlaque_World.Register();
            ProtoLogo_World.Register();
            DamagedProtoLogo_World.Register();
            TeleporterTerminal_World.Register();
        }

        private void RegisterEncyEntries()
        {
            #region Precursor Ingot
            LanguageHandler.SetLanguageLine("DownloadedData/Precursor/Scan", "PrecursorIngotEncyLine");

            string title = Language.main.Get("PrecursorIngotEncyTitle");
            string description = Language.main.Get("PrecursorIngotEncyBody");

            PDAHandler.AddEncyclopediaEntry("PrecursorIngot", "DownloadedData/Precursor/Scan", title, description, unlockSound: PDAHandler.UnlockBasic);
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

            StoryGoalHandler.RegisterCompoundGoal("Ency_PrecursorIngot", Story.GoalType.Encyclopedia, 12f, "Goal_BiomePrecursorGunUpper");

            StoryGoalHandler.RegisterCustomEvent("Ency_PrecursorIngot", () =>
            {
                KnownTech.Add(PrecursorIngot_Craftable.prefabInfo.TechType);
                PDAEncyclopedia.Add("PrecursorIngot", true);
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