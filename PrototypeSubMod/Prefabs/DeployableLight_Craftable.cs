using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Utility;
using SubLibrary.Handlers;
using System.Collections;
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

        prefab.SetGameObject(GetPrefab);
        prefab.SetRecipeFromJson(Path.Combine(Plugin.RecipesFolderPath, "DeployableLight.json"))
            .WithFabricatorType(CraftTree.Type.Fabricator)
            .WithStepsToFabricatorTab("Machines")
            .WithCraftingTime(10f);

        prefab.SetEquipment(Plugin.LightBeaconEquipmentType);
        prefab.SetPdaGroupCategory(TechGroup.Machines, TechCategory.Machines);

        prefab.Register();
    }

    private static IEnumerator GetPrefab(IOut<GameObject> prefabOut)
    {
        var assetPrefab = Plugin.AssetBundle.LoadAsset<GameObject>("DeployableLight");

        assetPrefab.SetActive(false);
        var prefab = GameObject.Instantiate(assetPrefab);

        prefab.GetComponent<Pickupable>().isPickupable = false;

        yield return new WaitUntil(() => MaterialUtils.IsReady);

        yield return CyclopsReferenceHandler.EnsureCyclopsReference();

        MaterialUtils.ApplySNShaders(prefab);

        InterfaceCallerHandler.InvokeCyclopsReferencers(prefab);

        prefabOut.Set(prefab);
    }
}
