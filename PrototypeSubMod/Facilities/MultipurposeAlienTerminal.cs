using System;
using System.Collections;
using UnityEngine;
using UWE;

namespace PrototypeSubMod.Facilities;

internal class MultipurposeAlienTerminal : MonoBehaviour
{
    public event Action onTerminalInteracted;

    public string primaryTooltip = "GenericConsole";
    public string secondaryTooltip = "Tooltip_GenericConsole";

    private IEnumerator Start()
    {
        var prefabRequest = PrefabDatabase.GetPrefabAsync("d200d747-b802-43f4-80b1-5c3d2155fbcd");

        yield return prefabRequest;

        GameObject prefab = null;
        if (!prefabRequest.TryGetPrefab(out prefab)) throw new Exception("Error retrieving alien terminal prefab");

        var instance = Instantiate(prefab, transform, false);
        instance.transform.localPosition = Vector3.zero;
        instance.transform.localRotation = Quaternion.identity;
        instance.transform.localScale = Vector3.one;

        var storyTarget = instance.GetComponentInChildren<StoryHandTarget>();
        var protoTarget = storyTarget.gameObject.EnsureComponent<ProtoTerminalHandTarget>();

        protoTarget.destroyGameObject = storyTarget.destroyGameObject;
        protoTarget.informGameObject = storyTarget.informGameObject;
        protoTarget.primaryTooltip = primaryTooltip;
        protoTarget.secondaryTooltip = secondaryTooltip;
    }
}
