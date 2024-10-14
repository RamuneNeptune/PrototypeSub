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
            .WithStepsToFabricatorTab("Resources", "AdvancedMaterials")
            .WithCraftingTime(10f);

        prefab.SetEquipment(Plugin.PrototypePowerType);
        prefab.SetPdaGroupCategory(TechGroup.Resources, TechCategory.AdvancedMaterials);

        prefab.Register();
    }

    private static IEnumerator GetPrefab(IOut<GameObject> prefabOut)
    {
        var assetPrefab = Plugin.AssetBundle.LoadAsset<GameObject>("DeployableLight");

        assetPrefab.SetActive(false);
        var prefab = GameObject.Instantiate(assetPrefab);

        yield return new WaitUntil(() => MaterialUtils.IsReady);

        Plugin.Logger.LogInfo($"Prism = {prefab} | Model = {assetPrefab}");
        MaterialUtils.ApplySNShaders(prefab, modifiers: new ProtoMaterialModifier(3f));

        prefabOut.Set(prefab);
    }
}
