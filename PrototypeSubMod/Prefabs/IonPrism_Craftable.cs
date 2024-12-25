using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Utility;
using PrototypeSubMod.Compatibility;
using PrototypeSubMod.Utility;
using System.Collections;
using UnityEngine;

namespace PrototypeSubMod.Prefabs;

internal class IonPrism_Craftable
{
    public static PrefabInfo prefabInfo { get; private set; }

    public static void Register()
    {
        prefabInfo = PrefabInfo.WithTechType("IonPrism", null, null)
            .WithIcon(Plugin.AssetBundle.LoadAsset<Sprite>("IonPrism_Icon"));

        var prefab = new CustomPrefab(prefabInfo);

        prefab.SetGameObject(GetPrefab);

        prefab.SetRecipe(ROTACompatManager.GetRelevantRecipe("IonPrism.json"))
            .WithFabricatorType(CraftTree.Type.Fabricator)
            .WithStepsToFabricatorTab("Resources", "AdvancedMaterials")
            .WithCraftingTime(10f);

        prefab.SetEquipment(Plugin.PrototypePowerType);
        prefab.SetPdaGroupCategory(TechGroup.Resources, TechCategory.AdvancedMaterials);

        CraftData.pickupSoundList.Add(prefabInfo.TechType, "event:/loot/pickup_precursorioncrystal");

        prefab.Register();
    }

    private static GameObject GetPrefab()
    {
        var assetPrefab = Plugin.AssetBundle.LoadAsset<GameObject>("IonPrism_Prefab");

        var prefab = GameObject.Instantiate(assetPrefab);
        return prefab;
    }
}
