using System;
using System.Collections;
using UnityEngine;
using UWE;

namespace PrototypeSubMod.Facilities;

internal class MultipurposeAlienTerminal : MonoBehaviour
{
    public event Action onTerminalInteracted;
    private static GameObject prefab;

    public string primaryTooltip = "GenericConsole";
    public string secondaryTooltip = "Tooltip_GenericConsole";
    public bool allowMultipleUses;

    private bool queuedForceInteract;
    private ProtoTerminalHandTarget handTarget;

    private void Start()
    {
        CoroutineHost.StartCoroutine(SpawnTerminal());
    }

    private IEnumerator SpawnTerminal()
    {
        var prefabRequest = PrefabDatabase.GetPrefabAsync("d200d747-b802-43f4-80b1-5c3d2155fbcd");

        yield return prefabRequest;

        GameObject prefab = null;
        if (!prefabRequest.TryGetPrefab(out prefab)) throw new Exception("Error retrieving alien terminal prefab");

        prefab.SetActive(false);
        MultipurposeAlienTerminal.prefab = prefab;
        var instance = Instantiate(prefab, transform, false);
        instance.transform.localPosition = Vector3.zero;
        instance.transform.localRotation = Quaternion.identity;
        instance.transform.localScale = Vector3.one;
        instance.SetActive(true);

        var storyTarget = instance.GetComponentInChildren<StoryHandTarget>();
        handTarget = storyTarget.gameObject.EnsureComponent<ProtoTerminalHandTarget>();
        Destroy(instance.GetComponent<LargeWorldEntity>());
        Destroy(instance.GetComponent<PrefabIdentifier>());

        handTarget.destroyGameObject = storyTarget.destroyGameObject;
        handTarget.informGameObjects = new[] { storyTarget.informGameObject, gameObject };
        handTarget.primaryTooltip = primaryTooltip;
        handTarget.secondaryTooltip = secondaryTooltip;

        Destroy(storyTarget);

        if (queuedForceInteract)
        {
            GetComponentInChildren<PrecursorComputerTerminal>().OnStoryHandTarget();
            queuedForceInteract = false;
            handTarget.interactionAllowed = allowMultipleUses;
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
