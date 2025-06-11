using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace PrototypeSubMod.Teleporter;

internal class TeleporterReferenceAssigner : MonoBehaviour
{
    private const string TeleporterPrefabKey = "WorldEntities/Environment/Precursor/MountainIsland/Precursor_Mountain_Teleporter_ToFloatingIsland.prefab";

    [SerializeField] private PrecursorTeleporter teleporter;
    [SerializeField, Tooltip("Can be null if you don't want to swap the mesh")] private Mesh fxMesh;
    [SerializeField] private float lightRangeOverride = -1;
    [SerializeField] private Vector3 lightLocalPos;

    private void OnValidate()
    {
        if (TryGetComponent(out PrecursorTeleporter tp)) teleporter = tp;
    }

    private void Start()
    {
        UWE.CoroutineHost.StartCoroutine(Initialize());
    }

    private IEnumerator Initialize()
    {
        var operation = Addressables.LoadAssetAsync<GameObject>(TeleporterPrefabKey);

        yield return operation;

        var prefab = operation.Result;

        var precursorTp = prefab.GetComponent<PrecursorTeleporter>();
        GameObject fxPrefab = precursorTp.portalFxPrefab;
        fxPrefab = Instantiate(fxPrefab, new Vector3(0, 5000, 0), Quaternion.identity); // Hide it in the sky
        fxPrefab.name = "x_PrecursorTeleporter_LargePortal";

        if (fxMesh != null)
        {
            fxPrefab.GetComponentInChildren<MeshFilter>().mesh = fxMesh;
        }

        var light = fxPrefab.GetComponentInChildren<Light>();
        light.range = lightRangeOverride < 0 ? light.range : 5f;
        light.transform.localPosition = lightLocalPos;

        teleporter.portalFxPrefab = fxPrefab;
        teleporter.cinematicEndControllerPrefabReference = precursorTp.cinematicEndControllerPrefabReference;

        teleporter.Start();
    }
}
