using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Utility;
using PrototypeSubMod.Utility;
using System.Collections;
using System.IO;
using UnityEngine;

namespace PrototypeSubMod.Prefabs;

internal class PrecursorIngot_Craftable
{
    public static PrefabInfo prefabInfo { get; private set; }

    public static void Register()
    {
        prefabInfo = PrefabInfo.WithTechType("Proto_PrecursorIngot", null, null, "English")
            .WithIcon(SpriteManager.Get(TechType.PlasteelIngot))
            .WithSizeInInventory(new Vector2int(2, 2))
            .WithIcon(Plugin.AssetBundle.LoadAsset<Sprite>("AlienFramework_Icon"));

        var prefab = new CustomPrefab(prefabInfo);

        prefab.SetGameObject(GetPrefab);

        prefab.SetRecipeFromJson(Path.Combine(Plugin.RecipesFolderPath, "Normal\\Proto_PrecursorIngot.json"))
                .WithFabricatorType(CraftTree.Type.Fabricator)
                .WithStepsToFabricatorTab("Resources", "AdvancedMaterials")
                .WithCraftingTime(10f);

        prefab.SetPdaGroupCategory(TechGroup.Resources, TechCategory.BasicMaterials);

        prefab.Register();
    }

    private static IEnumerator GetPrefab(IOut<GameObject> prefabOut)
    {
        var prefab = Plugin.AssetBundle.LoadAsset<GameObject>("AlienFramework");
        prefab.SetActive(false);

        var instance = GameObject.Instantiate(prefab);
        yield return new WaitUntil(() => MaterialUtils.IsReady);

        MaterialUtils.ApplySNShaders(prefab, modifiers: new ProtoMaterialModifier(3f));
        prefabOut.Set(instance);
    }
}
