using Nautilus.Assets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Handlers;
using Nautilus.Utility;
using PrototypeSubMod.MiscMonobehaviors.Materials;
using UnityEngine;

namespace PrototypeSubMod.Registration;

internal static class BiomeRegisterer
{
    public static void Register()
    {
        var settings = BiomeUtils.CreateBiomeSettings(new Vector3(18, 15, 13), 1.1f, Color.white, 0.15f, Color.white, 0, temperature: 10);

        BiomeHandler.RegisterBiome(Plugin.DEFENSE_CHAMBER_BIOME_NAME, settings, new BiomeHandler.SkyReference("SkyMountains"));
        PrefabInfo volumePrefabInfo = PrefabInfo.WithTechType("ProtoDefenseFacilityBiomeVolume");
        CustomPrefab volumePrefab = new CustomPrefab(volumePrefabInfo);
        AtmosphereVolumeTemplate template = new AtmosphereVolumeTemplate(volumePrefabInfo, AtmosphereVolumeTemplate.VolumeShape.Cube,
            Plugin.DEFENSE_CHAMBER_BIOME_NAME, 15, LargeWorldEntity.CellLevel.Global);
        template.ModifyPrefab = prefab =>
        {
            var volum = prefab.GetComponent<AtmosphereVolume>();
            prefab.AddComponent<AtmospherePriorityEnsurer>().priority = volum.priority;
        };

        volumePrefab.SetGameObject(template);
        volumePrefab.Register();

        var spawnInfo = new SpawnInfo(volumePrefabInfo.ClassID, new Vector3(710f, -375f, -1493f), Quaternion.identity, new Vector3(250, 800, 300));
        CoordinatedSpawnsHandler.RegisterCoordinatedSpawn(spawnInfo);

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
            BiomeUtils.CreateBiomeSettings(Vector3.zero, 0f, Color.white, 0.01f, new Color(0.824f, 0.922f, 0.828f), 0, startDistance: 0);
        BiomeHandler.RegisterBiome("protohullfacilitycalm", hullSettings, new BiomeHandler.SkyReference("SkyPrecursorInterior_NoLightmaps"));
        BiomeHandler.AddBiomeMusic("protohullfacilitycalm",
            AudioUtils.GetFmodAsset("HullFacility_Calm"));

        BiomeHandler.RegisterBiome("protohullfacilitytense", hullSettings, new BiomeHandler.SkyReference("SkyPrecursorInterior_NoLightmaps"));
        BiomeHandler.AddBiomeMusic("protohullfacilitytense",
            AudioUtils.GetFmodAsset("HullFacility_Tense"));
        #endregion
    }
}
