using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Utility;
using PrototypeSubMod.Compatibility;
using PrototypeSubMod.Utility;
using SubLibrary.Handlers;
using SubLibrary.Monobehaviors;
using System.Collections;
using UnityEngine;

namespace PrototypeSubMod.Prefabs;

internal class Prototype_Craftable
{
    public static PrefabInfo SubInfo { get; private set; }

    public static void Register()
    {
        PrefabInfo prefabInfo = PrefabInfo.WithTechType("PrototypeSub", null, null, "English")
            .WithIcon(SpriteManager.Get(TechType.PrecursorIonBattery));

        SubInfo = prefabInfo;

        var prefab = new CustomPrefab(prefabInfo);

        prefab.RemoveFromCache();
        prefab.SetGameObject(GetSubPrefab);
        prefab.SetUnlock(TechType.Constructor);

        prefab.SetRecipe(ROTACompatManager.GetRelevantRecipe("PrototypeSub.json"))
            .WithFabricatorType(CraftTree.Type.None)
            .WithCraftingTime(20f);

        prefab.SetPdaGroupCategory(Plugin.PrototypeGroup, Plugin.PrototypeCategory);

        prefab.Register();
    }

    private static IEnumerator GetSubPrefab(IOut<GameObject> prefabOut)
    {
        GameObject model = Plugin.AssetBundle.LoadAsset<GameObject>("PrototypeSub");

        model.SetActive(false);
        GameObject prototype = GameObject.Instantiate(model);

        yield return new WaitUntil(() => MaterialUtils.IsReady);

        MaterialUtils.ApplySNShaders(prototype, modifiers: new ProtoMaterialModifier(15f, 0));

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
