using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using PrototypeSubMod.Compatibility;
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
            .WithSizeInInventory(new Vector2int(2, 1));

        var prefab = new CustomPrefab(prefabInfo);

        var cloneTemplate = new CloneTemplate(prefabInfo, TechType.PlasteelIngot);

        cloneTemplate.ModifyPrefab += gameObject =>
        {
            foreach (var rend in gameObject.GetComponentsInChildren<Renderer>(true))
            {
                rend.material.color = new Color(0.25f, 1f, 0.25f);
            }
        };

        prefab.SetGameObject(cloneTemplate);

        if (!ROTACompatManager.ArchitectsLibInstalled)
        {
            prefab.SetRecipeFromJson(Path.Combine(Plugin.RecipesFolderPath, "Normal\\Proto_PrecursorIngot.json"))
                .WithFabricatorType(CraftTree.Type.Fabricator)
                .WithStepsToFabricatorTab("Resources", "AdvancedMaterials")
                .WithCraftingTime(10f);

            prefab.SetPdaGroupCategory(TechGroup.Resources, TechCategory.BasicMaterials);
        }

        prefab.Register();
    }
}
