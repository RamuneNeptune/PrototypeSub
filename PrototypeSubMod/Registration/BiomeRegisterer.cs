using Nautilus.Assets.PrefabTemplates;
using Nautilus.Assets;
using Nautilus.Handlers;
using Nautilus.Utility;
using UnityEngine;
using PrototypeSubMod.MiscMonobehaviors.Materials;

namespace PrototypeSubMod.Registration;

internal class BiomeRegisterer
{
    public static void Register()
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
        //BiomeHandler.AddBiomeMusic("interceptorisland", AudioUtils.GetFmodAsset("DefenseTunnelMusic1"), FMODGameParams.InteriorState.OnlyOutside);
        #endregion
    }
}
