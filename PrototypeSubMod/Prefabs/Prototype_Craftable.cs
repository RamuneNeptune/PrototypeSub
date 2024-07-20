using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Utility;
using SubLibrary.Handlers;
using SubLibrary.Monobehaviors;
using System.Collections;
using System.IO;
using UnityEngine;

namespace PrototypeSubMod.Prefabs;

internal class Prototype_Craftable
{
    public static PrefabInfo SubInfo { get; private set; }

    public static void Register()
    {
        PrefabInfo prefabInfo = PrefabInfo.WithTechType("PrototypeSub", null, null, "English")
            .WithIcon(SpriteManager.Get(TechType.PrecursorDroid));

        SubInfo = prefabInfo;

        var prefab = new CustomPrefab(prefabInfo);

        prefab.RemoveFromCache();
        prefab.SetGameObject(GetSubPrefab);
        prefab.SetUnlock(TechType.Constructor);

        prefab.SetRecipeFromJson(Path.Combine(Plugin.RecipesFolderPath, "Prototype.json"))
            .WithFabricatorType(CraftTree.Type.Constructor)
            .WithStepsToFabricatorTab("Vehicles")
            .WithCraftingTime(20f);

        prefab.SetPdaGroupCategory(TechGroup.Constructor, TechCategory.Constructor);

        prefab.Register();
    }

    private static IEnumerator GetSubPrefab(IOut<GameObject> prefabOut)
    {
        GameObject model = Plugin.AssetBundle.LoadAsset<GameObject>("PrototypeSub");

        model.SetActive(false);
        GameObject prototype = GameObject.Instantiate(model);

        yield return new WaitUntil(() => MaterialUtils.IsReady);

        MaterialUtils.ApplySNShaders(prototype);

        yield return CyclopsReferenceHandler.EnsureCyclopsReference();

        yield return InterfaceCallerHandler.InvokeCyclopsReferencers(prototype);

        foreach (var modifier in prototype.GetComponentsInChildren<PrefabModifier>(true))
        {
            modifier.OnAsyncPrefabTasksCompleted();
            modifier.OnLateMaterialOperation();
        }

        prefabOut.Set(prototype);
    }
}
