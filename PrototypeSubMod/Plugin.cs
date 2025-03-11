using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using EpicStructureLoader;
using HarmonyLib;
using ModStructureFormat;
using Nautilus.Assets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Handlers;
using Nautilus.Utility;
using Newtonsoft.Json;
using PrototypeSubMod.Commands;
using PrototypeSubMod.Compatibility;
using PrototypeSubMod.MiscMonobehaviors.Materials;
using PrototypeSubMod.Patches;
using PrototypeSubMod.PowerSystem;
using PrototypeSubMod.Prefabs;
using PrototypeSubMod.Prefabs.FacilityProps;
using PrototypeSubMod.PrototypeStory;
using PrototypeSubMod.SaveData;
using PrototypeSubMod.Upgrades;
using PrototypeSubMod.Utility;
using SubLibrary.Audio;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
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
        private const string versionString = "0.13.2";

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

        public static PingType DefenseFacilityPingType = EnumHandler.AddEntry<PingType>("DefenseFacility").WithIcon(AssetBundle.LoadAsset<Sprite>("DefenseFacilityLogo"));

        internal static ProtoGlobalSaveData GlobalSaveData = SaveDataHandler.RegisterSaveDataCache<ProtoGlobalSaveData>();
        internal static GameObject welderPrefab;

        internal static Sprite PrototypeSaveIcon = AssetBundle.LoadAsset<Sprite>("ProtoSaveIcon");
        internal static PingType PrototypePingType = EnumHandler.AddEntry<PingType>("PrototypeSub")
            .WithIcon(AssetBundle.LoadAsset<Sprite>("Proto_HUD_Marker"));

        internal const string DEFENSE_CHAMBER_BIOME_NAME = "protodefensefacility";
        internal static readonly Vector3 STORY_END_POS = new Vector3(-1333, -900, -3014);
        internal static readonly Vector3 DEFENSE_PING_POS = new Vector3(701, -366, -1359);
        internal static TechType DefenseFacilityPingTechType;

        private static bool Initialized;
        private static Harmony harmony = new Harmony(GUID);

        private void Awake()
        {
            if (Initialized) return;

            // Set project-scoped logger instance
            Logger = base.Logger;

            LanguageHandler.RegisterLocalizationFolder();
            SubAudioLoader.LoadAllAudio(AssetBundle);

            RegisterPrefabs();
            RegisterEncyEntries();
            RegisterStructures();
            RegisterStoryGoals();
            RegisterBiomes();
            RegisterCommands();
            RegisterPDAMessages();
            RegisterDependantPatches();
            InitializeSlotMapping();

            ConsoleCommandsHandler.RegisterConsoleCommands(typeof(PrototypeCommands));
            LoadEasyPrefabs.LoadPrefabs(AssetBundle);
            ROTACompatManager.AddCompatiblePowerSources();
            WeatherCompatManager.Initialize();

            UWE.CoroutineHost.StartCoroutine(Initialize());

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
            ProtoEngineFacilityRoom.Register();
            PrecursorCross.Register();
            PrecursorRadio.Register();
            InterceptorIslandTeleporterKey_World.Register();
            DefenseStoryGoalTrigger_World.Register();

            ProtoPlaque_World.Register();
            ProtoLogo_World.Register();
            DamagedProtoLogo_World.Register();
            TeleporterTerminal_World.Register();
            SmashedDisplayCase_World.Register();
            NonScannableProp.Register("11e731e7-bc82-4f94-90be-5db7b58b449b", "EmptyDisplayCase");
            NonScannableProp.Register("4f5905f8-ea50-49e8-b24f-44139c6bddcf", "PrecursorScannerArmNoScan1");
            NonScannableProp.Register("ebc943e4-200c-4789-92f3-e675cd982dbe", "PrecursorScannerArmNoScan2");
            NonScannableProp.Register("ac2b0798-e311-4cb1-9074-fae59cd7347a", "PrecursorScannerArmNoScan3");
            NonScannableProp.Register("d3645d71-518d-4546-9b68-a3352b07399a", "EmptyMultiDisplayCase");
            KinematicPrefabClone.Register(IonPrism_Craftable.prefabInfo.ClassID, "KinematicIonPrism");
            KinematicPrefabClone.Register("4af48036-40ba-46b1-a398-ede0bb106213", "KinematicLavaBoomerang");
            KinematicPrefabClone.Register("5f6d9ad1-540d-44b1-b62d-2478cd041ae5", "KinematicLavaEyeEye");
            KinematicPrefabClone.Register("a9da9324-84ed-4a51-9ed3-a0969f455067", "KinematicPeeper");
            KinematicPrefabClone.Register("0db5b44d-19f1-4349-9e1f-04da097010f3", "KinematicBoomerang");
            KinematicPrefabClone.Register("b1d88c87-fd48-495b-a707-e91dc4259858", "KinematicHoverfish");
            KinematicPrefabClone.Register("5de7d617-c04c-4a83-b663-ebf1d3dd90a1", "KinematicGarryfish");
            KinematicPrefabClone.Register("ba851576-86df-48e5-a0be-5cd7ba6f4617", "KinematicSpadefish");
            KinematicPrefabClone.Register("38ebd2e5-9dcc-4d7a-ada4-86a22e01191a", "KinematicIonCrystal");
            KinematicPrefabClone.Register("f90d7d3c-d017-426f-af1a-62ca93fae22e", "KinematicIonCrystalMatrix");

            DisplayCaseProp.Register(IonPrism_Craftable.prefabInfo.ClassID, "IonPrism_DisplayCase",
                IonPrism_Craftable.prefabInfo.TechType, new Vector3(0, 1.3f, 0), Vector3.one * 10f);
            DisplayCaseProp.Register(DeployableLight_Craftable.prefabInfo.ClassID, "DeployableLight_DisplayCase",
                DeployableLight_Craftable.prefabInfo.TechType, new Vector3(0, 1.3f, 0), Vector3.one * 0.25f, new[] { "VolumetricLight" });

            DefenseFacilityPingTechType = CustomPing.CreatePing("ProtoDefenseFacilityPing", DefenseFacilityPingType, new Color(1, 0, 0));

            Texture2D dogIco = AssetBundle.LoadAsset<Texture2D>("dogPosterIcon");
            new CustomPoster("ProtoDogPoster", null, null, AssetBundle.LoadAsset<Texture2D>("DogPoster"), dogIco);
            Texture2D regular1Ico = AssetBundle.LoadAsset<Texture2D>("RegularIcon1");
            new CustomPoster("HamCheesePoster1", null, null, AssetBundle.LoadAsset<Texture2D>("HamAndCheesePoster1_Small"), regular1Ico);
            Texture2D regular2Ico = AssetBundle.LoadAsset<Texture2D>("RegularIcon2");
            new CustomPoster("HamCheesePoster2", null, null, AssetBundle.LoadAsset<Texture2D>("RegularPoster2"), regular2Ico, TechType.PosterExoSuit1);
        }

        private void RegisterEncyEntries()
        {
            #region Prototype
            string protoTitle = Language.main.Get("ProtoDatabankEncy_Title");
            string protoBody = Language.main.Get("ProtoDatabankEncy_Body");
            PDAHandler.AddEncyclopediaEntry("ProtoDatabankEncy", "DownloadedData/Precursor/Terminal", protoTitle, protoBody, unlockSound: PDAHandler.UnlockImportant);
            #endregion

            #region Precursor Ingot
            string ingotTitle = Language.main.Get("ProtoPrecursorIngotEncy_Title");
            string ingotDescription = Language.main.Get("ProtoPrecursorIngotEncy_Body");
            var ingotPopup = AssetBundle.LoadAsset<Sprite>("AlienFramework_EncyPopup");

            PDAHandler.AddEncyclopediaEntry("ProtoPrecursorIngot", "DownloadedData/Precursor/Scan", ingotTitle, ingotDescription, unlockSound: PDAHandler.UnlockBasic, popupImage: ingotPopup);
            #endregion

            #region Interceptor Terminal
            LanguageHandler.SetLanguageLine("DownloadedData/Precursor/Terminal", "ProtoTeleporterEncyEntry");

            string protoTeleporterText = Language.main.Get("ProtoTeleporterEncyEntry");
            string protoTeleporterTextBody = Language.main.Get("ProtoTeleporterEncyEntry_Body");
            Texture2D image = AssetBundle.LoadAsset<Texture2D>("ProtoTeleporter_Corrupted");

            PDAHandler.AddEncyclopediaEntry("InterceptorTestEncy", "DownloadedData/Precursor/Scan", protoTeleporterText, protoTeleporterTextBody, image, unlockSound: PDAHandler.UnlockBasic);
            #endregion

            #region Deployable Light
            string lightTitle = Language.main.Get("ProtoDeployableLightEncy_Title");
            string lightDescription = Language.main.Get("ProtoDeployableLightEncy_Body");
            var lightPopup = AssetBundle.LoadAsset<Sprite>("DeployableLight_EncyPopup");

            PDAHandler.AddEncyclopediaEntry("ProtoDeployableLightEncy", "Tech/Equipment", lightTitle, lightDescription, unlockSound: PDAHandler.UnlockBasic, popupImage: lightPopup);
            var deployableLightEntryData = new PDAScanner.EntryData()
            {
                key = DeployableLight_Craftable.prefabInfo.TechType,
                destroyAfterScan = false,
                encyclopedia = "ProtoDeployableLightEncy",
                scanTime = 5f,
                isFragment = false,
                blueprint = DeployableLight_Craftable.prefabInfo.TechType
            };
            PDAHandler.AddCustomScannerEntry(deployableLightEntryData);
            #endregion

            #region Ion Prism
            string prismTitle = Language.main.Get("ProtoIonPrismEncy_Title");
            string prismDescription = Language.main.Get("ProtoIonPrismEncy_Body");
            var prismPopup = AssetBundle.LoadAsset<Sprite>("IonPrism_EncyPopup");

            PDAHandler.AddEncyclopediaEntry("ProtoIonPrismEncy", "DownloadedData/Precursor/Scan", prismTitle, prismDescription, unlockSound: PDAHandler.UnlockBasic, popupImage: prismPopup);
            var ionPrismEntryData = new PDAScanner.EntryData()
            {
                key = IonPrism_Craftable.prefabInfo.TechType,
                destroyAfterScan = false,
                encyclopedia = "ProtoIonPrismEncy",
                scanTime = 5f,
                isFragment = false,
                blueprint = IonPrism_Craftable.prefabInfo.TechType
            };
            PDAHandler.AddCustomScannerEntry(ionPrismEntryData);
            #endregion

            #region Orion Logs
            string orionTitle = Language.main.Get("OrionFacilityLogs_Title");
            string orionBody = Language.main.Get("OrionFacilityLogs_Body");
            PDAHandler.AddEncyclopediaEntry("OrionFacilityLogsEncy", "DownloadedData/Precursor/Terminal", orionTitle, orionBody, unlockSound: PDAHandler.UnlockBasic);
            #endregion

            #region Defense Audit Logs
            string auditTitle = Language.main.Get("DefenseFacilityLogs_Title");
            string auditBody = Language.main.Get("DefenseFacilityLogs_Body");
            PDAHandler.AddEncyclopediaEntry("DefenseFacilityAuditEncy", "DownloadedData/Precursor/Terminal", auditTitle, auditBody, unlockSound: PDAHandler.UnlockBasic);
            #endregion

            #region Facility Locations
            string locationsTitle = Language.main.Get("ProtoFacilitiesEncy_Title");
            string locationsBody = Language.main.Get("ProtoFacilitiesEncy_Body");
            PDAHandler.AddEncyclopediaEntry("ProtoFacilitiesEncy", "DownloadedData/Precursor/Terminal", locationsTitle, locationsBody, unlockSound: PDAHandler.UnlockBasic);
            #endregion

            RegisterEncyEntries("DownloadedData/Precursor/ProtoUpgrades", PDAHandler.UnlockBasic, new()
            {
                "ProtoCloakEncy",
                "ProtoEmergencyWarpEncy",
                "ProtoInterceptorEncy",
                "ProtoRepairDroidsEncy",
                "ProtoPressureConvertersEncy",
                "ProtoIonBarrierEncy",
                "ProtoStasisPulseEncy",
                "ProtoVentilatorsEncy",
                "ProtoIonGeneratorEncy",
                "ProtoOverclockEncy"
            });
        }

        private void RegisterStoryGoals()
        {
            #region Precursor Ingot

            StoryGoalHandler.RegisterItemGoal("Ency_ProtoPrecursorIngot", Story.GoalType.Encyclopedia, PrecursorIngot_Craftable.prefabInfo.TechType);

            StoryGoalHandler.RegisterCustomEvent("Ency_ProtoPrecursorIngot", () =>
            {
                KnownTech.Add(PrecursorIngot_Craftable.prefabInfo.TechType);
                PDAEncyclopedia.Add("ProtoPrecursorIngot", true);
            });

            #endregion

            #region PPT First Interaction
            PPTStoryManager.RegisterGoals();
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

            #region Disable Defense Cloak
            StoryGoalHandler.RegisterCustomEvent("OnDefenseCloakDisabled", () =>
            {
                PDALog.Add("OnDefenseCloakDisabled");
            });
            #endregion

            #region Moonpool Enter
            StoryGoalHandler.RegisterCustomEvent("OnEnterDefenseMoonpool", () =>
            {
                PDALog.Add("OnEnterDefenseMoonpool");
            });

            StoryGoalHandler.RegisterLocationGoal("OnEnterDefenseMoonpool", Story.GoalType.PDA, new Vector3(819, -463, -1115), 15, 0);
            #endregion

            #region Moonpool Open Disallowed
            StoryGoalHandler.RegisterCustomEvent("OnMoonpoolNoPrototype", () =>
            {
                PDALog.Add("OnMoonpoolNoPrototype");
            });
            #endregion

            #region On Approach Defense Beacon
            StoryGoalHandler.RegisterCustomEvent("OnApproachDefenseFacility", () =>
            {
                PDALog.Add("OnApproachDefenseFacility");
            });
            #endregion

            #region Orion Logs
            StoryGoalHandler.RegisterCustomEvent("Ency_OrionFacilityLogs", () =>
            {
                PDAEncyclopedia.Add("OrionFacilityLogsEncy", true);
            });
            #endregion

            #region Defense Audit Logs
            StoryGoalHandler.RegisterCompoundGoal("DefenseFacilityAuditEncy", Story.GoalType.Encyclopedia, 7f, "OnDisableDefenseCloak");

            StoryGoalHandler.RegisterCustomEvent("DefenseFacilityAuditEncy", () =>
            {
                KnownTech.Add(PrecursorIngot_Craftable.prefabInfo.TechType);
                PDAEncyclopedia.Add("DefenseFacilityAuditEncy", true);
            });
            #endregion
        }

        private void RegisterStructures()
        {
            int entityCount = 0;
            //StructureLoading.RegisterStructure(LoadStructureFromBundle("InterceptorFacility"), ref entityCount);
            //entityCount = 0;

            StructureLoading.RegisterStructure(LoadStructureFromBundle("DefenseChamber"), ref entityCount);
            entityCount = 0;

            StructureLoading.RegisterStructure(LoadStructureFromBundle("DefenseTunnel"), ref entityCount);
            entityCount = 0;

            /*
            StructureLoading.RegisterStructure(LoadStructureFromBundle("EngineFacility"), ref entityCount);
            entityCount = 0;
            */

            StructureLoading.RegisterStructure(LoadStructureFromBundle("ProtoIslands"), ref entityCount);
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

            BiomeHandler.AddBiomeMusic(DEFENSE_CHAMBER_BIOME_NAME, AudioUtils.GetFmodAsset("DefenseFacilityExterior"));

            #region Tunnel Biomes
            var tunnelSettings = BiomeUtils.CreateBiomeSettings(new Vector3(20, 20, 20), 1f, Color.white, 0.12f, Color.white, 0, startDistance: 20);

            BiomeHandler.RegisterBiome("protodefensetunnel1", tunnelSettings, new BiomeHandler.SkyReference("SkyCrashZone"));
            BiomeHandler.AddBiomeMusic("protodefensetunnel1", AudioUtils.GetFmodAsset("DefenseTunnelMusic1"), FMODGameParams.InteriorState.OnlyOutside);
            BiomeHandler.RegisterBiome("protodefensetunnel2", tunnelSettings, new BiomeHandler.SkyReference("SkyCrashZone"));
            BiomeHandler.AddBiomeMusic("protodefensetunnel2", AudioUtils.GetFmodAsset("DefenseTunnelMusic2"), FMODGameParams.InteriorState.OnlyOutside);
            BiomeHandler.RegisterBiome("protodefensetunnel3", tunnelSettings, new BiomeHandler.SkyReference("SkyCrashZone"));
            BiomeHandler.AddBiomeMusic("protodefensetunnel3", AudioUtils.GetFmodAsset("DefenseTunnelMusic3"), FMODGameParams.InteriorState.OnlyOutside);
            BiomeHandler.RegisterBiome("protodefensetunnel4", tunnelSettings, new BiomeHandler.SkyReference("SkyCrashZone"));
            BiomeHandler.AddBiomeMusic("protodefensetunnel4", AudioUtils.GetFmodAsset("DefenseTunnelMusic4"), FMODGameParams.InteriorState.OnlyOutside);
            BiomeHandler.RegisterBiome("protodefensetunnel5", tunnelSettings, new BiomeHandler.SkyReference("SkyCrashZone"));
            BiomeHandler.AddBiomeMusic("protodefensetunnel5", AudioUtils.GetFmodAsset("DefenseTunnelMusic5"), FMODGameParams.InteriorState.OnlyOutside);
            #endregion

            #region Interceptor Island
            var islandSettings = BiomeUtils.CreateBiomeSettings(new Vector3(40, 15, 9), 0.4f, Color.white, 0.12f, Color.white, 0, 25, 1.4f);
            BiomeHandler.RegisterBiome("interceptorisland", islandSettings, new BiomeHandler.SkyReference("SkyCrashZone"));
            //BiomeHandler.AddBiomeMusic("interceptorisland", AudioUtils.GetFmodAsset("DefenseTunnelMusic1"), FMODGameParams.InteriorState.OnlyOutside);
            #endregion
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

        private void RegisterCommands()
        {
            ConsoleCommandsHandler.AddGotoTeleportPosition("interceptorfacility", new Vector3(547, -709, 955));
            ConsoleCommandsHandler.AddGotoTeleportPosition("defensefacility", new Vector3(689, -483, -1404f));
            ConsoleCommandsHandler.AddGotoTeleportPosition("enginefacility", new Vector3(306, -1156, 131f));
        }

        private void RegisterPDAMessages()
        {
            PDALog_Patches.entries.Add(("PDA_InterceptorUnlock", "OnInterceptorTestDataDownloaded"));
            PDALog_Patches.entries.Add(("PDA_OnDisableCloak", "OnDefenseCloakDisabled"));
            PDALog_Patches.entries.Add(("PDA_OnEnterMoonpool", "OnEnterDefenseMoonpool"));
            PDALog_Patches.entries.Add(("PDA_OnMoonpoolDisallow", "OnMoonpoolNoPrototype"));
            PDALog_Patches.entries.Add(("PDA_ApproachDefense", "OnApproachDefenseFacility"));
        }

        private Structure LoadStructureFromBundle(string name)
        {
            var structureFile = AssetBundle.LoadAsset<TextAsset>(name);
            if (!structureFile)
            {
                return new Structure(new Entity[0]);
            }

            return JsonConvert.DeserializeObject<Structure>(structureFile.text);
        }

        private void RegisterEncyEntries(string path, FMODAsset unlockSound, List<string> entries)
        {
            foreach (var entry in entries)
            {
                string title = Language.main.Get($"{entry}_Title");
                string body = Language.main.Get($"{entry}_Body");
                PDAHandler.AddEncyclopediaEntry(entry, path, title, body, unlockSound: unlockSound);
            }
        }
    }
}