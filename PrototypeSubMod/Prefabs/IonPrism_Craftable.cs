using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Utility;
using PrototypeSubMod.Compatibility;
using PrototypeSubMod.Utility;
using System.Collections;
using System.IO;
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
            .WithStepsToFabricatorTab("Resources", "AdvancedMaterials")
            .WithCraftingTime(10f);

        prefab.SetPdaGroupCategory(TechGroup.Resources, TechCategory.AdvancedMaterials);

        prefab.Register();
    }

    private static IEnumerator GetPrefab(IOut<GameObject> prefabOut)
    {
        var model = Plugin.AssetBundle.LoadAsset<GameObject>("IonPrism");

        model.SetActive(false);
        var prism = GameObject.Instantiate(model);

        yield return new WaitUntil(() => MaterialUtils.IsReady);

        MaterialUtils.ApplySNShaders(prism, modifiers: new ProtoMaterialModifier(3f, 0));

        prefabOut.Set(prism);
    }
}
