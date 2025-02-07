using System.Collections;
using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.PrefabRetrievers;

internal class SpawnSeaglideMap : MonoBehaviour
{
    private static GameObject SeaglidePrefab;

    [SerializeField] private float mapScale = 1f;
    [SerializeField] private float fadeRadius = 0.6f;
    [SerializeField] private float fadeSharpness = 5f;

    private IEnumerator Start()
    {
        if (SeaglidePrefab)
        {
            SpawnMap();
            yield break;
        }

        var seaglideTask = CraftData.GetPrefabForTechTypeAsync(TechType.Seaglide);
        yield return seaglideTask;

        SeaglidePrefab = seaglideTask.GetResult();
        SpawnMap();
    }

    private void SpawnMap()
    {
        var mapController = SeaglidePrefab.GetComponentInChildren<VehicleInterface_MapController>();
        var mapObject = Instantiate(mapController.interfacePrefab);
        mapObject.transform.SetParent(transform, false);
        var miniWorld = mapObject.GetComponentInChildren<MiniWorld>();
        miniWorld.active = true;
        miniWorld.hologramRadius = mapScale * 150f / 10f;
        miniWorld.fadeRadius = fadeRadius;
        miniWorld.fadeSharpness = fadeSharpness;
    }
}
