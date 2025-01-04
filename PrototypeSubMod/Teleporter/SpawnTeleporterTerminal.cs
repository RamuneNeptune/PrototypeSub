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

        var terminal = Instantiate(prefab, transform, false);
        var placeholder = terminal.GetComponentInChildren<PrefabPlaceholder>();

        DestroyImmediate(placeholder.gameObject);

        terminal.transform.position = transform.position;
        terminal.transform.rotation = transform.rotation;

        var activationComponent = terminal.GetComponent<PrecursorTeleporterActivationTerminal>();
        activationComponent.root = teleporterRoot;
        activationComponent.onUseGoal.key = "";
        activationComponent.gameObject.AddComponent<ProtoKeyTerminalTrigger>();

        var trigger = terminal.GetComponentInChildren<PrecursorKeyTerminalTrigger>();
        var newTrigger = trigger.gameObject.AddComponent<ProtoTeleporterTerminalTrigger>();
        Destroy(trigger);

        var identifier = terminal.GetComponent<PrefabIdentifier>();
        Destroy(identifier);

        newTrigger.SetSubRoot(GetComponentInParent<SubRoot>());
        newTrigger.GetComponent<SphereCollider>().radius = 1.75f;
    }
}
