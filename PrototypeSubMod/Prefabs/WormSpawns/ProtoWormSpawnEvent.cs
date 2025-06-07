using System.Collections;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Utility;
using PrototypeSubMod.Utility;
using UnityEngine;

namespace PrototypeSubMod.Prefabs.WormSpawns;

public class ProtoWormSpawnEvent
{
    private static GameObject lastPrefab;
    
    public static PrefabInfo RegisterEvent(string classID, GameObject prefab, LootDistributionData.BiomeData[] spawnLocations)
    {
        var prefabInfo = PrefabInfo.WithTechType(classID, null, null);

        var customPrefab = new CustomPrefab(prefabInfo);

        lastPrefab = prefab;
        customPrefab.SetGameObject(GetPrefabInstance);
        customPrefab.SetSpawns(spawnLocations);
        customPrefab.RemoveFromCache();

        customPrefab.Register();
        return prefabInfo;
    }

    private static IEnumerator GetPrefabInstance(IOut<GameObject> prefabOut)
    {
        var instance = UWE.Utils.InstantiateDeactivated(lastPrefab);
        MaterialUtils.ApplySNShaders(instance);
        yield return ProtoMatDatabase.ReplaceVanillaMats(instance);
        
        prefabOut.Set(instance);
    }
}