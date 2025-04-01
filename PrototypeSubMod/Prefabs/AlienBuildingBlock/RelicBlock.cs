using System.Collections;
using UnityEngine;
using UWE;

namespace PrototypeSubMod.Prefabs.AlienBuildingBlock;

internal class RelicBlock
{
    
    protected static IEnumerator GetRelicBlockModel(IOut<GameObject> blockModel)
    {
        var task = PrefabDatabase.GetPrefabAsync("09bc9a07-7680-4ddf-9ba2-a7da5e7b3287");
        yield return task;

        if (!task.TryGetPrefab(out var relicBlock))
        {
            Plugin.Logger.LogError("Failed to load the RelicBlock prefab.");
            blockModel.Set(null);
            yield break;
        }

        var meshRenderer = relicBlock.GetComponentInChildren<MeshRenderer>();
        
        blockModel.Set(meshRenderer.gameObject);
    }
    
}