using HarmonyLib;
using PrototypeSubMod.Utility;
using System;
using System.Collections;
using UnityEngine;
using UWE;

namespace PrototypeSubMod.Facilities;

internal class MultipurposeAlienTerminal : MonoBehaviour
{
    public event Action onTerminalInteracted;
    [SaveStateReference]
    private static GameObject prefab;

    [SerializeField] private string primaryTooltip = "GenericConsole";
    [SerializeField] private string secondaryTooltip = "Tooltip_GenericConsole";
    [SerializeField] private bool allowMultipleUses;
    [SerializeField] private bool spawnWithLight = true;

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

        handTarget.informGameObjects = new GameObject[] { handTarget.gameObject, gameObject };
        handTarget.primaryTooltip = primaryTooltip;
        handTarget.secondaryTooltip = secondaryTooltip;

        if (queuedForceInteract)
        {
            var terminal = GetComponentInChildren<PrecursorComputerTerminal>();
            CoroutineHost.StartCoroutine(QueuedForceInteract(terminal));
            queuedForceInteract = false;
            handTarget.interactionAllowed = allowMultipleUses;
        }

        if (!spawnWithLight)
        {
            foreach (var item in GetComponentsInChildren<Light>())
            {
                Destroy(item);
            }
        }

        var applier = GetComponentInParent<SkyApplier>();
        if (applier)
        {
            applier.renderers.AddRangeToArray(GetComponentsInChildren<Renderer>(true));
            applier.ApplySkybox();
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
        if (!terminal || !terminal.fxControl || !terminal.scaleControl)
        {
            queuedForceInteract = true;
            return;
        }

        terminal.OnStoryHandTarget();
    }

    private IEnumerator QueuedForceInteract(PrecursorComputerTerminal terminal)
    {
        int frameCount = 0;
        while (terminal.fxControl == null)
        {
            yield return new WaitForEndOfFrame();
            frameCount++;

            if (frameCount > 60) yield break;
        }

        ForceInteracted();
    }
}
