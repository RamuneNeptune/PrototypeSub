using BepInEx;
using BepInEx.Logging;
using EpicStructureLoader;
using HarmonyLib;
using ModStructureFormat;
using Nautilus.Assets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Handlers;
using Nautilus.Utility;
using Newtonsoft.Json;
using PrototypeSubMod.Compatibility;
using PrototypeSubMod.Monobehaviors;
using PrototypeSubMod.PowerSystem;
using PrototypeSubMod.Prefabs;
using PrototypeSubMod.Prefabs.FacilityProps;
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
    [BepInDependency("com.lee23.theredplague", BepInDependency.DependencyFlags.SoftDependency)]
    public class Plugin : BaseUnityPlugin
    {
        private const string GUID = "com.prototech.prototypesub";
        private const string pluginName = "Prototype Sub";
        private const string versionString = "0.0.8.1";

        public new static ManualLogSource Logger { get; private set; }

        internal static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

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

        internal static ProtoGlobalSaveData GlobalSaveData = SaveDataHandler.RegisterSaveDataCache<ProtoGlobalSaveData>();
        internal static GameObject welderPrefab;

        internal static Sprite PrototypeSaveIcon = AssetBundle.LoadAsset<Sprite>("ProtoSaveIcon");

        private static bool Initialized;

        private void Awake()
        {
            if (Initialized) return;

            // Set project-scoped logger instance
            Logger = base.Logger;
            
            LanguageHandler.RegisterLocalizationFolder();
            SubAudioLoader.LoadAllAudio(AssetBundle);
            
            RegisterEncyEntries();
            RegisterStructures();
            RegisterPrefabs();
            RegisterStoryGoals();
            RegisterBiomes();
            RegisterCommands();
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
            DeactivatedTeleporter_World.Register();

            ProtoPlaque_World.Register();
            ProtoLogo_World.Register();
            DamagedProtoLogo_World.Register();
            TeleporterTerminal_World.Register();

            Texture2D dogIco = AssetBundle.LoadAsset<Texture2D>("dogPosterIcon");
            new CustomPoster("ProtoDogPoster", null, null, AssetBundle.LoadAsset<Texture2D>("DogPoster"), dogIco);
            Texture2D regular1Ico = AssetBundle.LoadAsset<Texture2D>("RegularIcon1");
            new CustomPoster("HamCheesePoster1", null, null, AssetBundle.LoadAsset<Texture2D>("HamAndCheesePoster1_Small"), regular1Ico);
            Texture2D regular2Ico = AssetBundle.LoadAsset<Texture2D>("RegularIcon2");
            new CustomPoster("HamCheesePoster2", null, null, AssetBundle.LoadAsset<Texture2D>("RegularPoster2"), regular2Ico, TechType.PosterExoSuit1);
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
            var interceptorStructureFile = AssetBundle.LoadAsset<TextAsset>("ProtoInterceptorFacility");
            var interceptorStructure = JsonConvert.DeserializeObject<Structure>(interceptorStructureFile.text);

            int entityCount = 0;
            StructureLoading.RegisterStructure(interceptorStructure, ref entityCount);
            entityCount = 0;

            if (TRPCompatManager.TRPInstalled)
            {
                var trpIslandFile = AssetBundle.LoadAsset<TextAsset>("RedPlagueProtoIslands");
                var trpIsland = JsonConvert.DeserializeObject<Structure>(trpIslandFile.text);

                StructureLoading.RegisterStructure(trpIsland, ref entityCount);
            }
        }

        private void RegisterBiomes()
        {
            var settings = BiomeUtils.CreateBiomeSettings(new Vector3(18, 15, 13), 1.1f, Color.white, 0.15f, Color.white, 0, temperature: 10);

            BiomeHandler.RegisterBiome("protodefensefacility", settings, new BiomeHandler.SkyReference("SkyMountains"));
            PrefabInfo volumePrefabInfo = PrefabInfo.WithTechType("ProtoDefenseFacilityBiomeVolume");
            CustomPrefab volumePrefab = new CustomPrefab(volumePrefabInfo);
            AtmosphereVolumeTemplate template = new(volumePrefabInfo, AtmosphereVolumeTemplate.VolumeShape.Cube,
                "protodefensefacility", 15, LargeWorldEntity.CellLevel.Global);
            template.ModifyPrefab = prefab =>
            {
                var volum = prefab.GetComponent<AtmosphereVolume>();
                prefab.AddComponent<AtmospherePriorityEnsurer>().priority = volum.priority;
            };

            volumePrefab.SetGameObject(template);
            volumePrefab.Register();

            var spawnInfo = new SpawnInfo(volumePrefabInfo.ClassID, new Vector3(710f, -375f, -1493f), Quaternion.identity, new Vector3(250, 800, 300));
            CoordinatedSpawnsHandler.RegisterCoordinatedSpawn(spawnInfo);

            BiomeHandler.AddBiomeMusic("protodefensefacility", AudioUtils.GetFmodAsset("DefenseFacilityExterior"));
        }

        private void RegisterCommands()
        {
            ConsoleCommandsHandler.AddGotoTeleportPosition("interceptorfacility", new Vector3(547, -709, 955));
            ConsoleCommandsHandler.AddGotoTeleportPosition("defensefacility", new Vector3(689, -483, -1404f));
            ConsoleCommandsHandler.AddGotoTeleportPosition("enginefacility", new Vector3(306, -1156, 131f));
        }
    }
}