using PrototypeSubMod.Utility;
using System.Collections;
using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.PrefabRetrievers;

internal class SpawnSeaglideMap : MonoBehaviour
{
    [SaveStateReference]
    private static GameObject SeaglidePrefab;

    [SerializeField] private MeshRenderer positionDot;
    [SerializeField] private Color mapColor = new Color(0.23f, 0.57f, 0.85f);
    [SerializeField] private float mapScale = 1f;
    [SerializeField] private float fadeRadius = 0.6f;
    [SerializeField] private float fadeSharpness = 5f;

    private MiniWorld miniWorld;

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

        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        miniWorld.materialInstance.SetColor(ShaderPropertyID._Color, mapColor);
        miniWorld.mapColor = mapColor;
        miniWorld.mapColorNoAlpha = new Color(mapColor.r, mapColor.g, mapColor.b, 0);
    }

    private void SpawnMap()
    {
        var pingRend = SeaglidePrefab.transform.Find("MapHolder/PlayerPing/Ping").GetComponent<Renderer>();

        if (positionDot)
        {
            var color = positionDot.material.color;
            var tex = positionDot.material.mainTexture;
            positionDot.material = new(pingRend.material);
            positionDot.material.color = color;
            positionDot.material.mainTexture = tex;
        }

        var mapController = SeaglidePrefab.GetComponentInChildren<VehicleInterface_MapController>();
        var mapObject = Instantiate(mapController.interfacePrefab);
        mapObject.transform.SetParent(transform, false);
        miniWorld = mapObject.GetComponentInChildren<MiniWorld>();
        miniWorld.active = true;
        miniWorld.hologramRadius = mapScale * 150f / 10f;
        miniWorld.fadeRadius = fadeRadius;
        miniWorld.fadeSharpness = fadeSharpness;
    }
}
