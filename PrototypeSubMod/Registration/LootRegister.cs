using Nautilus.Handlers;
using Nautilus.Utility;

namespace PrototypeSubMod.Registration;

internal static class LootRegister
{

    public static void Register()
    {
        RegisterInactiveAlienBuildingBlock();
    }

    private static void RegisterInactiveAlienBuildingBlock()
    {
        UWE.Utils.TryParseEnum<TechType>("InactiveAlienBuildingBlock", out var inactiveBuildingBlockType);
        
        var worldEntityInfo = WorldEntityInfoUtils.Create(inactiveBuildingBlockType.ToString(),  inactiveBuildingBlockType, LargeWorldEntity.CellLevel.Far, EntitySlot.Type.Small);
        LootDistributionHandler.AddLootDistributionData(inactiveBuildingBlockType.ToString(), worldEntityInfo, new LootDistributionData.BiomeData[] {
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
                probability = 0.2f
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