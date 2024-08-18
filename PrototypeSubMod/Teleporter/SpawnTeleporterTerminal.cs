using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace PrototypeSubMod.Teleporter;

internal class SpawnTeleporterTerminal : MonoBehaviour
{
    [SerializeField] private GameObject teleporterRoot;
    [SerializeField] private string addressableKey;

    private IEnumerator Start()
    {
        var operation = Addressables.LoadAssetAsync<GameObject>(addressableKey);

        yield return operation;

        var prefab = operation.Result;

        var terminal = Instantiate(prefab, transform);
        terminal.transform.position = transform.position;
        terminal.transform.rotation = transform.rotation;

        terminal.GetComponent<PrecursorTeleporterActivationTerminal>().root = teleporterRoot;
    }
}
