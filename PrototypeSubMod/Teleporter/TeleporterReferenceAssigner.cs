using System.Collections;
using System.Threading;
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
        if (fxMesh != null)
        {
            fxPrefab = Instantiate(fxPrefab);
            fxPrefab.GetComponentInChildren<MeshFilter>().mesh = fxMesh;
        }

        teleporter.portalFxPrefab = fxPrefab;
        teleporter.cinematicEndControllerPrefabReference = precursorTp.cinematicEndControllerPrefabReference;

        teleporter.Start();
    }
}
