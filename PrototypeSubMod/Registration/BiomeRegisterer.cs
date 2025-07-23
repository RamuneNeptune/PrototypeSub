﻿using Nautilus.Assets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Handlers;
using Nautilus.Utility;
using PrototypeSubMod.MiscMonobehaviors.Materials;
using PrototypeSubMod.MiscMonobehaviors.SubSystems;
using UnityEngine;

namespace PrototypeSubMod.Registration;

internal static class BiomeRegisterer
{
    public static void Register()
    {
        var sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        
        var settings = BiomeUtils.CreateBiomeSettings(new Vector3(18, 15, 13), 1.1f, Color.white, 0.15f, Color.white, 0, temperature: 10);

        BiomeHandler.RegisterBiome(Plugin.DEFENSE_CHAMBER_BIOME_NAME, settings, new BiomeHandler.SkyReference("SkyMountains"));
        BiomeHandler.AddBiomeMusic(Plugin.DEFENSE_CHAMBER_BIOME_NAME, AudioUtils.GetFmodAsset("DefenseFacilityExterior"));
        
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
        BiomeHandler.AddBiomeMusic("interceptorisland", AudioUtils.GetFmodAsset("ProtoIslandMusic"), FMODGameParams.InteriorState.OnlyOutside);
        #endregion

        #region Engine Facility

        var engineSettings = BiomeUtils.CreateBiomeSettings(new Vector3(35, 7f, 5.5f), 0.4f, Color.white, 0.15f,
            Color.clear,
            1f, 25, 1, 1, 24);
        BiomeHandler.RegisterBiome(Plugin.ENGINE_FACILITY_BIOME_NAME, engineSettings, new BiomeHandler.SkyReference("SkyBloodKelpTwo"));
        BiomeHandler.AddBiomeMusic(Plugin.ENGINE_FACILITY_BIOME_NAME, AudioUtils.GetFmodAsset("EngineFacilityMusic"));

        #endregion

        #region Hull Facility
        var hullSettings =
            BiomeUtils.CreateBiomeSettings(new Vector3(16, 12, 6), 2f, new Color(0, 1, 0.912f), 
                0.25f, new Color(0, 0.95f, 1),
                0.03f, 40, 0.5f, 20f);
        BiomeHandler.RegisterBiome("protohullfacilitycalm", hullSettings, new BiomeHandler.SkyReference("SkyPrecursorInterior_NoLightmaps"));
        BiomeHandler.AddBiomeMusic("protohullfacilitycalm",
            AudioUtils.GetFmodAsset("HullFacility_Calm"));

        BiomeHandler.RegisterBiome("protohullfacilitytense", hullSettings, new BiomeHandler.SkyReference("SkyPrecursorInterior_NoLightmaps"));
        BiomeHandler.AddBiomeMusic("protohullfacilitytense",
            AudioUtils.GetFmodAsset("HullFacility_Tense"));
        #endregion

        #region Story Ping Void
        
        PrefabInfo voidVolumePrefabInfo = PrefabInfo.WithTechType("StoryPingVoidBiome");
        CustomPrefab voidVolumePrefab = new CustomPrefab(voidVolumePrefabInfo);
        AtmosphereVolumeTemplate voidTemplate = new AtmosphereVolumeTemplate(voidVolumePrefabInfo, AtmosphereVolumeTemplate.VolumeShape.Sphere,
            "void", 11, LargeWorldEntity.CellLevel.Global);
        voidTemplate.ModifyPrefab = prefab =>
        {
            var volum = prefab.GetComponent<AtmosphereVolume>();
            prefab.AddComponent<AtmospherePriorityEnsurer>().priority = volum.priority;
            prefab.AddComponent<DestroyOnStoryEnd>();
        };

        voidVolumePrefab.SetGameObject(voidTemplate);
        voidVolumePrefab.Register();

        var voidSpawnInfo = new SpawnInfo(voidVolumePrefabInfo.ClassID, Plugin.STORY_END_POS, Quaternion.identity, Vector3.one * 2400);
        CoordinatedSpawnsHandler.RegisterCoordinatedSpawn(voidSpawnInfo);

        #endregion

        sw.Stop();
        Plugin.Logger.LogInfo($"Biomes registered in {sw.ElapsedMilliseconds}ms");
    }
}
