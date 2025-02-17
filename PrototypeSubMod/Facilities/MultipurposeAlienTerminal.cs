using System;
using System.Collections;
using System.Linq;
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
        if (prefab != null)
        {
            SetupTerminal(prefab);
            return;
        }

        CoroutineHost.StartCoroutine(RetrievePrefab());
    }

    private IEnumerator RetrievePrefab()
    {
        var prefabRequest = PrefabDatabase.GetPrefabAsync("PrototypeGenericTerminal");

        yield return prefabRequest;

        GameObject prefab = null;
        if (!prefabRequest.TryGetPrefab(out prefab)) throw new Exception("Error retrieving alien terminal prefab");

        prefab.SetActive(false);
        MultipurposeAlienTerminal.prefab = prefab;
        SetupTerminal(prefab);
    }

    private void SetupTerminal(GameObject prefab)
    {
        var instance = Instantiate(prefab, transform, false);
        instance.transform.localPosition = Vector3.zero;
        instance.transform.localRotation = Quaternion.identity;
        instance.transform.localScale = Vector3.one;
        instance.SetActive(true);

        handTarget = instance.EnsureComponent<ProtoTerminalHandTarget>();
        Destroy(instance.GetComponent<LargeWorldEntity>());
        Destroy(instance.GetComponent<PrefabIdentifier>());

        handTarget.informGameObjects.Append(gameObject);
        handTarget.primaryTooltip = primaryTooltip;
        handTarget.secondaryTooltip = secondaryTooltip;

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
