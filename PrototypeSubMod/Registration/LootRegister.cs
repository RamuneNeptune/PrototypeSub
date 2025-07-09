using Nautilus.Handlers;
using Nautilus.Utility;
using PrototypeSubMod.Prefabs.AlienBuildingBlock;

namespace PrototypeSubMod.Registration;

internal static class LootRegister
{

    public static void Register()
    {
        var sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        RegisterInactiveAlienBuildingBlock();

        sw.Stop();
        Plugin.Logger.LogInfo($"Loot spawns registered in {sw.ElapsedMilliseconds}ms");
    }

    private static void RegisterInactiveAlienBuildingBlock()
    {
        var worldEntityInfo = WorldEntityInfoUtils.Create(WarperRemnant.prefabInfo.TechType.ToString(),  WarperRemnant.prefabInfo.TechType, LargeWorldEntity.CellLevel.Near, EntitySlot.Type.Small);
        LootDistributionHandler.AddLootDistributionData(WarperRemnant.prefabInfo.TechType.ToString(), worldEntityInfo, new LootDistributionData.BiomeData[] {
            new()
            {
                biome = BiomeType.GrandReef_Ground,
                count = 1,
                probability = 0.3f
            },
            new()
            {
                biome = BiomeType.DeepGrandReef_Ground,
                count = 1,
                probability = 0.3f
            },
            new()
            {
                biome = BiomeType.Mountains_IslandCaveFloor,
                count = 1,
                probability = 0.05f
            },
            new()
            {
                biome = BiomeType.SeaTreaderPath_Path,
                count = 1,
                probability = 0.2f
            },
            new()
            {
                biome = BiomeType.BloodKelp_TrenchFloor,
                count = 1,
                probability = 0.2f
            }
        });
    }
    
}