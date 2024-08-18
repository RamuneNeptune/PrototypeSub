using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace PrototypeSubMod.Teleporter;

internal class TeleporterReferenceAssigner : MonoBehaviour
{
    private const string TeleporterPrefabKey = "WorldEntities/Environment/Precursor/MountainIsland/Precursor_Mountain_Teleporter_ToFloatingIsland.prefab";

    [SerializeField] private PrecursorTeleporter teleporter;
    [SerializeField, Tooltip("Can be null if you don't want to swap the mesh")] private Mesh fxMesh;

    private void OnValidate()
    {
        if (TryGetComponent(out PrecursorTeleporter tp)) teleporter = tp;
    }

    private IEnumerator Start()
    {
        var operation = Addressables.LoadAssetAsync<GameObject>(TeleporterPrefabKey);

        yield return operation;

        var prefab = operation.Result;

        var precursorTp = prefab.GetComponent<PrecursorTeleporter>();
        GameObject fxPrefab = precursorTp.portalFxPrefab;
        fxPrefab = Instantiate(fxPrefab, new Vector3(0, 5000, 0), Quaternion.identity); // Hide it in the sky

        if (fxMesh != null)
        {
            fxPrefab.GetComponentInChildren<MeshFilter>().mesh = fxMesh;
        }

        var light = fxPrefab.GetComponentInChildren<Light>();
        light.range = 5f;
        light.transform.localPosition = new Vector3(0, 1.5f, -2f);

        teleporter.portalFxPrefab = fxPrefab;
        teleporter.cinematicEndControllerPrefabReference = precursorTp.cinematicEndControllerPrefabReference;

        teleporter.Start();
    }
}
