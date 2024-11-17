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
    public bool allowMultipleUses;

    private bool queuedForceInteract;
    private ProtoTerminalHandTarget handTarget;

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
        handTarget = storyTarget.gameObject.EnsureComponent<ProtoTerminalHandTarget>();

        handTarget.destroyGameObject = storyTarget.destroyGameObject;
        handTarget.informGameObjects = new[] { storyTarget.informGameObject, gameObject };
        handTarget.primaryTooltip = primaryTooltip;
        handTarget.secondaryTooltip = secondaryTooltip;

        Destroy(storyTarget);

        if (queuedForceInteract)
        {
            GetComponentInChildren<PrecursorComputerTerminal>().OnStoryHandTarget();
            queuedForceInteract = false;
        }
    }

    public void OnStoryHandTarget()
    {
        onTerminalInteracted?.Invoke();
        handTarget.interactionAllowed = allowMultipleUses;
    }

    public void ForceInteracted()
    {
        var terminal = GetComponentInChildren<PrecursorComputerTerminal>();
        if (!terminal)
        {
            queuedForceInteract = true;
            return;
        }
        
        terminal.OnStoryHandTarget();
    }
}
