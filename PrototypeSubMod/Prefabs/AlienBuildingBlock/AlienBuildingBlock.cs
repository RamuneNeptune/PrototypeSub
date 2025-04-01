using System.Collections;
using UnityEngine;
using UWE;

namespace PrototypeSubMod.Prefabs.AlienBuildingBlock;

internal class AlienBuildingBlock
{
    
    protected static IEnumerator GetAlienBuildingBlockModel(IOut<GameObject> blockModel)
    {
        var task = PrefabDatabase.GetPrefabAsync("09bc9a07-7680-4ddf-9ba2-a7da5e7b3287");
        yield return task;

        if (!task.TryGetPrefab(out var alienRelic))
        {
            Plugin.Logger.LogError("Failed to load the RootRelic prefab.");
            blockModel.Set(null);
            yield break;
        }

        var meshRenderer = alienRelic.GetComponentInChildren<MeshRenderer>();
        
        blockModel.Set(meshRenderer.gameObject);
    }
    
}