using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace PrototypeSubMod.Teleporter;

internal class TeleporterReferenceAssigner : MonoBehaviour
{
    private const string TeleporterPrefabKey = "WorldEntities/Environment/Precursor/MountainIsland/Precursor_Mountain_Teleporter_ToFloatingIsland.prefab";

    [SerializeField] private PrecursorTeleporter teleporter;

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
        teleporter.portalFxPrefab = precursorTp.portalFxPrefab;
        teleporter.cinematicEndControllerPrefabReference = precursorTp.cinematicEndControllerPrefabReference;

        teleporter.Start();
    }
}
