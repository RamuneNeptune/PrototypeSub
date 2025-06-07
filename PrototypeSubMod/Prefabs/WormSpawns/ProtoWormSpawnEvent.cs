using System.Collections;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Utility;
using PrototypeSubMod.Utility;
using UnityEngine;

namespace PrototypeSubMod.Prefabs.WormSpawns;

public class ProtoWormSpawnEvent
{
    public static PrefabInfo RegisterEvent(string classID, GameObject prefab, LootDistributionData.BiomeData[] spawnLocations)
    {
        var prefabInfo = PrefabInfo.WithTechType(classID, null, null);

        var customPrefab = new CustomPrefab(prefabInfo);
        
        customPrefab.SetSpawns(spawnLocations);
        customPrefab.RemoveFromCache();

        UWE.CoroutineHost.StartCoroutine(GetPrefab(prefab, customPrefab));
        return prefabInfo;
    }

    private static IEnumerator GetPrefab(GameObject prefab, CustomPrefab customPrefab)
    {
        var instance = UWE.Utils.InstantiateDeactivated(prefab);
        MaterialUtils.ApplySNShaders(instance);
        yield return ProtoMatDatabase.ReplaceVanillaMats(instance);

        customPrefab.SetGameObject(instance);
        customPrefab.Register();
    }
}