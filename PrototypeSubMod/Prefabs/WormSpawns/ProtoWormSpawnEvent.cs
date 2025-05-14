using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Utility;
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

        customPrefab.Register();
        return prefabInfo;
    }

    private static GameObject GetPrefabInstance()
    {
        var instance = GameObject.Instantiate(lastPrefab);
        MaterialUtils.ApplySNShaders(instance);
        instance.SetActive(true);
        
        return instance;
    }
}