using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Crafting;
using Nautilus.Handlers;
using Newtonsoft.Json;
using PrototypeSubMod.Compatibility;
using System.IO;
using UnityEngine;

namespace PrototypeSubMod.Prefabs;

internal class CrystalMatrix_Craftable
{
    public static void Register()
    {
        string classID = "f90d7d3c-d017-426f-af1a-62ca93fae22e";
        string filePath = "WorldEntities/EnvironmentResources/PrecursorIonCrystalMatrix.prefab";
        PrefabInfo info = new PrefabInfo(classID, filePath, TechType.PrecursorIonCrystalMatrix);

        ICustomPrefab matrix = new CustomPrefab(info);
        var patch = new CustomPrefab("Proto_MatrixPlaceholder", "", "");
        patch.SetGameObject(() => GameObject.CreatePrimitive(PrimitiveType.Cube));
        patch.AddGadget(new ScanningGadget(matrix, Prototype_Craftable.SubInfo.TechType))
            .WithPdaGroupCategory(TechGroup.Resources, TechCategory.AdvancedMaterials);

        string text = File.ReadAllText(Path.Combine(Plugin.RecipesFolderPath, "PrecursorIonCrystalMatrix.json"));
        var recipeData = ROTACompatManager.SwapRecipeToCorrectIngot(text);
        patch.AddGadget(new CraftingGadget(matrix, recipeData)
            .WithCraftingTime(5f)
            .WithFabricatorType(CraftTree.Type.Fabricator)
            .WithStepsToFabricatorTab("Resources", "AdvancedMaterials"));

        Sprite matrixSprite = Plugin.AssetBundle.LoadAsset<Sprite>("matrixSprite");
        SpriteHandler.RegisterSprite(TechType.PrecursorIonCrystalMatrix, new Atlas.Sprite(matrixSprite));

        patch.Register();
    }
}
