using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using System.IO;
using UnityEngine;

namespace PrototypeSubMod.Prefabs;

internal class DeployableLight_Craftable
{
    public static PrefabInfo prefabInfo { get; private set; }

    public static void Register()
    {
        prefabInfo = PrefabInfo.WithTechType("DeployableLight", null, null, "English")
            .WithIcon(SpriteManager.Get(TechType.LEDLight));

        var prefab = new CustomPrefab(prefabInfo);

        var cloneTemplate = new CloneTemplate(prefabInfo, TechType.CyclopsDecoy);

        cloneTemplate.ModifyPrefab += gameObject =>
        {
            GameObject.Destroy(gameObject.GetComponent<CyclopsDecoy>());
            GameObject.Destroy(gameObject.GetComponent<EcoTarget>());

            foreach (var rend in gameObject.GetComponentsInChildren<Renderer>())
            {
                rend.material.color = new Color(0.5f, 1f, 0.5f);
            }
        };

        prefab.SetGameObject(cloneTemplate);

        prefab.SetRecipeFromJson(Path.Combine(Plugin.RecipesFolderPath, "DeployableLight.json"))
            .WithFabricatorType(CraftTree.Type.Fabricator)
            .WithStepsToFabricatorTab("Machines")
            .WithCraftingTime(10f);

        prefab.SetEquipment(Plugin.LightBeaconEquipmentType);
        prefab.SetPdaGroupCategory(TechGroup.Machines, TechCategory.Machines);

        prefab.Register();
    }
}
