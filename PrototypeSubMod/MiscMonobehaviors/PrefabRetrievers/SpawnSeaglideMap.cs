using System;
using PrototypeSubMod.Utility;
using System.Collections;
using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.PrefabRetrievers;

internal class SpawnSeaglideMap : MonoBehaviour
{
    [SaveStateReference]
    private static GameObject SeaglidePrefab;
    
    [SerializeField] private Color mapColor = new Color(0.23f, 0.57f, 0.85f);
    [SerializeField] private float mapScale = 1f;
    [SerializeField] private float fadeRadius = 0.6f;
    [SerializeField] private float fadeSharpness = 5f;
    [SerializeField] private Vector3 globalOffset;
    
    private MiniWorld miniWorld;

    private void Start()
    {
        if (SeaglidePrefab)
        {
            SpawnMap();
            return;
        }

        UWE.CoroutineHost.StartCoroutine(RetrievePrefab());
    }

    private IEnumerator RetrievePrefab()
    {
        var seaglideTask = CraftData.GetPrefabForTechTypeAsync(TechType.Seaglide);
        yield return seaglideTask;

        SeaglidePrefab = seaglideTask.GetResult();
        SpawnMap();

        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        if (!miniWorld.materialInstance)
        {
            miniWorld.InitializeHologram();
        }
        
        miniWorld.materialInstance.SetColor(ShaderPropertyID._Color, mapColor);
        miniWorld.mapColor = mapColor;
        miniWorld.mapColorNoAlpha = new Color(mapColor.r, mapColor.g, mapColor.b, 0);
        
        var miniWorldHolder = miniWorld.transform.Find("HologramHolder");
        miniWorldHolder.SetParent(transform, false);
    }

    private void Update()
    {
        if (!miniWorld) return;

        if (!miniWorld.hologramHolder) return;
        
        miniWorld.hologramHolder.position = transform.position + globalOffset;
    }

    private void SpawnMap()
    {
        var mapController = SeaglidePrefab.GetComponentInChildren<VehicleInterface_MapController>();
        var mapObject = Instantiate(mapController.interfacePrefab);
        mapObject.transform.SetParent(transform, false);
        miniWorld = mapObject.GetComponentInChildren<MiniWorld>();
        miniWorld.active = true;
        miniWorld.hologramRadius = mapScale * 150f / 10f;
        miniWorld.fadeRadius = fadeRadius;
        miniWorld.fadeSharpness = fadeSharpness;
        
        miniWorld.DisableMap();
    }
}
