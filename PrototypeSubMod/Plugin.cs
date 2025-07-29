using System;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
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
using Nautilus.Handlers.LoadingScreen;
using Nautilus.Handlers.TitleScreen;
using Nautilus.Utility;
using PrototypeSubMod.MiscMonobehaviors;
using PrototypeSubMod.Pathfinding.SaveSystem;
using PrototypeSubMod.Prefabs;
using PrototypeSubMod.Prefabs.AlienBuildingBlock;
using PrototypeSubMod.VehicleAccess;
using UnityEngine;
using UWE;

namespace PrototypeSubMod
{
    [BepInPlugin(GUID, pluginName, versionString)]
    [BepInDependency("com.snmodding.nautilus")]
    [BepInDependency("com.indigocoder.sublibrary")]
    [BepInDependency("com.alembic.package")]
    [BepInDependency("ArchitectsLibrary", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.lee23.theredplague", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.danithedani.deepercreatures", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.lee23.epicweather", BepInDependency.DependencyFlags.SoftDependency)]
    public class Plugin : BaseUnityPlugin
    {
        private const string GUID = "com.prototech.prototypesub";
        private const string pluginName = "Prototype Sub";
        private const string versionString = "1.0.0";

        public new static ManualLogSource Logger { get; private set; }

        internal static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

        public static string AssetsFolderPath { get; } = Path.Combine(Path.GetDirectoryName(Assembly.Location), "Assets");
        public static string RecipesFolderPath { get; } = Path.Combine(Path.GetDirectoryName(Assembly.Location), "Recipes");

        public static AssetBundle AssetBundle { get; private set; }
        public static AssetBundle AudioBundle { get; private set; }
        public static AssetBundle ScenesAssetBundle { get; private set; }
        public static AssetBundle TitleAssetBundle { get; } = AssetBundle.LoadFromFile(Path.Combine(AssetsFolderPath, "prototypetitle"));

        public static EquipmentType PrototypePowerType { get; } = EnumHandler.AddEntry<EquipmentType>("PrototypePowerType");
        public static EquipmentType LightBeaconEquipmentType { get; } = EnumHandler.AddEntry<EquipmentType>("LightBeaconType");
        public static EquipmentType DummyPowerType { get; } = EnumHandler.AddEntry<EquipmentType>("ProtoDummyPowerType");

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
        
        internal static PingType PrototypePingType { get; private set; }

        internal const string DEFENSE_CHAMBER_BIOME_NAME = "protodefensefacility";
        internal const string ENGINE_FACILITY_BIOME_NAME = "protoenginefacility";
        internal static readonly Vector3 STORY_END_POS = new Vector3(858, -800, 3116);
        internal static readonly Vector3 DEFENSE_PING_POS = new Vector3(700, -489, -1456);
        internal static TechType StoryEndPingTechType;
        internal static GridSaveData pathfindingGridSaveData;
        internal static event Action<GridSaveData> onLoadGridSaveData;

        private static bool Initialized;
        private static bool PrefabsInitialized;
        private static bool StructuresRegistered;
        private static bool MiscellaneousRegistered;
        private static Harmony harmony = new Harmony(GUID);

        private void Awake()
        {
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            // Set project-scoped logger instance
            Logger = base.Logger;
            
            // Register harmony patches, if there are any
            harmony.PatchAll(Assembly);

            UWE.CoroutineHost.StartCoroutine(LoadAudioAsync());
            UWE.CoroutineHost.StartCoroutine(LoadScenesBundle());

            var databaseSW = new System.Diagnostics.Stopwatch();
            databaseSW.Start();
            ProtoMatDatabase.Initalize();
            databaseSW.Stop();
            Logger.LogInfo($"Material database registered in {databaseSW.ElapsedMilliseconds}ms");
            
            LanguageHandler.RegisterLocalizationFolder();
            
            var voicelineSW = new System.Diagnostics.Stopwatch();
            voicelineSW.Start();
            VoicelineRegisterer.UpdateVoicelines();
            voicelineSW.Stop();
            Logger.LogInfo($"Voiceline variations registered in {sw.ElapsedMilliseconds}ms");
            RegisterDependentPatches();
            InitializeSlotMapping();
            RegisterTitleAddons();
            
            var miscSW = new System.Diagnostics.Stopwatch();
            miscSW.Start();
            ConsoleCommandsHandler.RegisterConsoleCommands(typeof(PrototypeCommands));
            WeatherCompatManager.Initialize();
            SetupSaveStateReferences.SetupReferences(Assembly);
            miscSW.Stop();
            Logger.LogInfo($"Miscellaneous items registered in {miscSW.ElapsedMilliseconds}ms");
            
            CoroutineHost.StartCoroutine(Initialize());
            CoroutineHost.StartCoroutine(MakeSeaTreaderBlockersPassthrough());
            CoroutineHost.StartCoroutine(LazyInitialize());

            var recipeData = CraftDataHandler.GetRecipeData(TechType.RocketStage3);
            for (int i = 0; i < recipeData.ingredientCount; i++)
            {
                var ingredient = recipeData.Ingredients[i];
                if (ingredient.techType == TechType.CyclopsShieldModule)
                {
                    ingredient = new CraftData.Ingredient(TechType.ReactorRod, 2);
                }

                recipeData.Ingredients[i] = ingredient;
            }
            
            CraftDataHandler.SetRecipeData(TechType.RocketStage3, recipeData);

            string modName = Language.main.Get("ProtoModName");
            WaitScreenHandler.RegisterAsyncLoadTask(modName, LoadBundleTask, Language.main.Get("ProtoWaitLoadingBundle"));
            WaitScreenHandler.RegisterAsyncLoadTask(modName, LoadPrefabsTask, Language.main.Get("ProtoWaitLoadingPrefabs"));
            WaitScreenHandler.RegisterAsyncLoadTask(modName, LoadStructuresTask, Language.main.Get("ProtoWaitRegisteringStructures"));
            WaitScreenHandler.RegisterAsyncLoadTask(modName, LoadMiscellaneousTask, Language.main.Get("ProtoWaitRegisteringMiscellaneous"));
            WaitScreenHandler.RegisterAsyncLoadTask(modName, LoadScenesBundle, Language.main.Get("ProtoWaitRegisteringScenes"));
            WaitScreenHandler.RegisterEarlyAsyncLoadTask(modName, LoadAudioBundle, Language.main.Get("ProtoWaitRegisteringAudio"));
            
            sw.Stop();
            Logger.LogInfo($"Plugin {GUID} is loaded in {sw.ElapsedMilliseconds} ms!");
        }

        private IEnumerator Initialize()
        {
            if (Initialized) yield break;
            
            Initialized = true;

            yield return new WaitUntil(() => CraftData.cacheInitialized && CraftTree.initialized);
            yield return new WaitForEndOfFrame();

            var task = CraftData.GetPrefabForTechTypeAsync(TechType.Welder);
            yield return task;

            welderPrefab = task.GetResult();

            var ghostTask = PrefabDatabase.GetPrefabAsync("54701bfc-bb1a-4a84-8f79-ba4f76691bef");
            yield return ghostTask;

            if (!ghostTask.TryGetPrefab(out var ghostPrefab)) throw new Exception("Error loading ghost leviathan prefab");

            ghostPrefab.EnsureComponent<GhostLeviathanFacilityManager>();

            if (Chainloader.PluginInfos.ContainsKey("com.aotu.returnoftheancients"))
            {
                var guardianTask = PrefabDatabase.GetPrefabAsync("GuardianConstruction_QEP");
                yield return guardianTask;
                
                if (!guardianTask.TryGetPrefab(out var guardianPrefab)) throw new Exception("Error loading RotA guardian prefab");

                guardianPrefab.EnsureComponent<DestroyOnStart>();
            }
        }

        private IEnumerator LoadBundleTask(WaitScreenHandler.WaitScreenTask waitTask)
        {

            waitTask.Status = Language.main.Get("ProtoWaitLoadingBundle");
            yield return new WaitUntil(() => AssetBundle != null);
        }

        private IEnumerator LoadPrefabsTask(WaitScreenHandler.WaitScreenTask waitTask)
        {
            waitTask.Status = Language.main.Get("ProtoWaitRegisteringPrefabs");
            yield return new WaitUntil(() => PrefabsInitialized);
        }

        private IEnumerator LoadStructuresTask(WaitScreenHandler.WaitScreenTask waitTask)
        {
            waitTask.Status = Language.main.Get("ProtoWaitRegisteringStructures");
            yield return new WaitUntil(() => StructuresRegistered);
        }

        private IEnumerator LoadMiscellaneousTask(WaitScreenHandler.WaitScreenTask waitTask)
        {
            waitTask.Status = Language.main.Get("ProtoWaitRegisteringMiscellaneous");
            yield return new WaitUntil(() => MiscellaneousRegistered);
        }

        private IEnumerator LoadAudioBundle(WaitScreenHandler.WaitScreenTask waitTask)
        {
            waitTask.Status = Language.main.Get("ProtoWaitRegisteringAudio");
            yield return new WaitUntil(() => AudioBundle != null);
            PDAMessageRegisterer.Register();
        }

        private IEnumerator LoadScenesBundle(WaitScreenHandler.WaitScreenTask waitTask)
        {
            waitTask.Status = Language.main.Get("ProtoWaitRegisteringScenes");
            yield return new WaitUntil(() => ScenesAssetBundle != null);
        }
        
        private IEnumerator LazyInitialize()
        {
            if (AssetBundle != null) yield break;
            
            var task = AssetBundle.LoadFromFileAsync(Path.Combine(AssetsFolderPath, "prototypeassets"));
            yield return task;
            AssetBundle = task.assetBundle;
            
            LoadPathfindingGrid();
            
            PrototypePingType = EnumHandler.AddEntry<PingType>("PrototypeSub")
                .WithIcon(AssetBundle.LoadAsset<Sprite>("Proto_HUD_Marker"));
            
            PrefabRegisterer.Register();
            yield return LoadEasyPrefabs.LoadPrefabs(AssetBundle, EncyEntryRegisterer.Register, GC.Collect, GC.WaitForPendingFinalizers);
            
            PrototypePowerSystem.AllowedPowerSources = new()
            {
                { WarperRemnant.prefabInfo.TechType, new PowerConfigData(2) },
                { AlienBuildingBlock.prefabInfo.TechType, new PowerConfigData(4) },
                { TechType.PrecursorIonCrystal, new PowerConfigData(5) },
                { EngineFacilityKey.prefabInfo.TechType, new PowerConfigData(6) },
                { TechType.PrecursorIonCrystalMatrix, new PowerConfigData(8) },
                { IonPrism_Craftable.prefabInfo.TechType, new PowerConfigData(10) }
            };
            
            ROTACompatManager.AddCompatiblePowerSources();

            yield return AddBatteryComponents();
            PrefabsInitialized = true;
            
            StructureRegisterer.Register();
            StructuresRegistered = true;
            
            StoryGoalsRegisterer.Register();
            BiomeRegisterer.Register();
            LootRegister.Register();
            CommandRegisterer.Register();
            MiscellaneousRegistered = true;
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

        private IEnumerator LoadScenesBundle()
        {
            var task = AssetBundle.LoadFromFileAsync(Path.Combine(AssetsFolderPath, "prototypescenes"));
            yield return task;
            ScenesAssetBundle = task.assetBundle;
        }

        private void RegisterTitleAddons()
        {
            #region Title Screen
            GameObject SpawnObject()
            {
                var worldObject = Instantiate(TitleAssetBundle.LoadAsset<GameObject>("ProtoPlaque"));
                worldObject.transform.position = new Vector3(-27, 2.5f, 38);
                worldObject.transform.rotation = Quaternion.Euler(270, 325.7f, 0);
                DestroyImmediate(worldObject.GetComponent<LargeWorldEntity>());
                DestroyImmediate(worldObject.GetComponent<PrefabIdentifier>());
                DestroyImmediate(worldObject.GetComponent<TechTag>());
                MaterialUtils.ApplySNShaders(worldObject);

                UWE.CoroutineHost.StartCoroutine(ProtoMatDatabase.ReplaceVanillaMats(worldObject));
                
                return worldObject;
            }
            
            var objectAddon = new WorldObjectTitleAddon(SpawnObject);
            var customData = new TitleScreenHandler.CustomTitleData("ProtoModName", objectAddon);

            const string addonName = "ProtoTitleData";
            TitleScreenHandler.RegisterTitleScreenObject(addonName, customData);
            #endregion

            #region Loading Screens

            var constructedData = new LoadingScreenHandler.LoadingScreenData(
                TitleAssetBundle.LoadAsset<Sprite>("ProtoCraftedScreen"), storyGoalRequirement: "PrototypeCrafted");
            var engineData = new LoadingScreenHandler.LoadingScreenData(
                TitleAssetBundle.LoadAsset<Sprite>("EngineFacilityScreen"), 2, storyGoalRequirement: "OnUnlocked_EngineUpgradeText_Native");
            var defenseData = new LoadingScreenHandler.LoadingScreenData(
                TitleAssetBundle.LoadAsset<Sprite>("DefenseFacilityScreen"), 3, storyGoalRequirement: "OnUnlocked_DefenseUpgradeText_Native");
            var interceptorData = new LoadingScreenHandler.LoadingScreenData(
                TitleAssetBundle.LoadAsset<Sprite>("ArchwayFacilityScreen"), 4, storyGoalRequirement: "OnUnlocked_ArchwayUpgradeText_Native");
            var hullData = new LoadingScreenHandler.LoadingScreenData(
                TitleAssetBundle.LoadAsset<Sprite>("HullFacilityScreen"), 5, storyGoalRequirement: "OnUnlocked_HullUpgradeText_Native");

            LoadingScreenHandler.RegisterLoadingScreen(addonName, new[]
            {
                constructedData,
                engineData,
                defenseData,
                interceptorData,
                hullData
            });

            #endregion
        }

        private void InitializeSlotMapping()
        {
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            
            foreach (string name in PrototypePowerSystem.SLOT_NAMES)
            {
                Equipment.slotMapping.Add(name, PrototypePowerType);
            }

            Equipment.slotMapping.Add(ProtoVehicleAccessTerminal.SLOT_NAME, EquipmentType.NuclearReactor);
            
            sw.Stop();
            Logger.LogInfo($"Slot mapping registered in {sw.ElapsedMilliseconds}ms");
        }

        private IEnumerator LoadAudioAsync()
        {
            var bundleRequest = AssetBundle.LoadFromFileAsync(Path.Combine(AssetsFolderPath, "prototypeaudio"));
            yield return bundleRequest;

            AudioBundle = bundleRequest.assetBundle;
            
            var audioSW = new System.Diagnostics.Stopwatch();
            audioSW.Start();
            var request = AudioBundle.LoadAllAssetsAsync(typeof(CustomFMODAsset));
            yield return request;
            
            foreach (var asset in request.allAssets)
            {
                SubAudioLoader.RegisterAssetAudio(asset as CustomFMODAsset);
            }
            audioSW.Stop();
            Logger.LogInfo($"Audio registered in {audioSW.ElapsedMilliseconds}ms");
        }

        private IEnumerator MakeSeaTreaderBlockersPassthrough()
        {
            var task = PrefabDatabase.GetPrefabAsync("626f6739-acb0-4dfc-bbab-9b627767403c");
            yield return task;

            task.TryGetPrefab(out var prefab);
            prefab.EnsureComponent<DontCollideWithPlayer>();
        }

        private void RegisterDependentPatches()
        {
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            
            if (Chainloader.PluginInfos.ContainsKey("com.danithedani.deepercreatures"))
            {
                var structureLoadingType =
                    Type.GetType(
                        "EpicStructureLoader.StructureLoading, EpicStructureLoader, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null");
                var structureTranspiler = AccessTools.Method(typeof(StructureLoading_Patches), nameof(StructureLoading_Patches.RegisterStructure_Transpiler));
                var originalMethod = AccessTools.Method(structureLoadingType, "RegisterStructure");
                harmony.Patch(originalMethod, transpiler: new HarmonyMethod(structureTranspiler));
            }

            if (Chainloader.PluginInfos.ContainsKey("com.mikjaw.subnautica.vehicleframework.mod"))
            {
                var smallEnoughPrefix = AccessTools.Method(typeof(VehicleFrameworkCompat_Patches),
                    nameof(VehicleFrameworkCompat_Patches.IsThisVehicleSmallEnough_Prefix));
                
                var handleDockedPrefix = AccessTools.Method(typeof(VehicleFrameworkCompat_Patches),
                    nameof(VehicleFrameworkCompat_Patches.HandleMVDocked_Prefix));

                var maybeStartPostfix = AccessTools.Method(typeof(VehicleFrameworkCompat_Patches),
                    nameof(VehicleFrameworkCompat_Patches.MaybeStartCinematicMode_Postfix));
                
                var onVehicleDockedTranspiler = AccessTools.Method(typeof(VehicleFrameworkCompat_Patches),
                    nameof(VehicleFrameworkCompat_Patches.OnVehicleDocked_Transpiler));

                var dockedPositionTranspiler = AccessTools.Method(typeof(VehicleFrameworkCompat_Patches),
                    nameof(VehicleFrameworkCompat_Patches.UpdateDockedPosition_Transpiler));
                
                var patchMethodType = Type.GetType("VehicleFramework.Patches.VehicleDockingBayPatch, VehicleFramework, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
                var patchMethodType2 = Type.GetType("VehicleFramework.Patches.VehicleDockingBayPatch2, VehicleFramework, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");

                var maybeStartMethod = AccessTools.Method(patchMethodType2, "MaybeStartCinematicMode");
                var smallEnoughMethod = AccessTools.Method(patchMethodType, "IsThisVehicleSmallEnough");
                var handleDockedMethod = AccessTools.Method(patchMethodType, "HandleMVDocked");
                var dockedPositionMethod = AccessTools.Method(patchMethodType, "UpdateDockedPositionPrefix");
                var onDockedMethod =
                    AccessTools.Method(VehicleFrameworkCompat_Patches.ModVehicleType, "OnVehicleDocked");
                
                harmony.Patch(smallEnoughMethod, prefix: new HarmonyMethod(smallEnoughPrefix));
                harmony.Patch(handleDockedMethod, prefix: new HarmonyMethod(handleDockedPrefix));
                harmony.Patch(maybeStartMethod, postfix: new HarmonyMethod(maybeStartPostfix));
                harmony.Patch(onDockedMethod, transpiler: new HarmonyMethod(onVehicleDockedTranspiler));
                harmony.Patch(dockedPositionMethod, transpiler: new HarmonyMethod(dockedPositionTranspiler));
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