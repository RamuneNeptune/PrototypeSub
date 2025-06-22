using System;
using System.Collections;
using HarmonyLib;
using PrototypeSubMod.MiscMonobehaviors.Materials;
using UnityEngine;
using UnityEngine.Events;

namespace PrototypeSubMod.Facilities;

public class MultipurposeIonCubeTerminal : InteractableTerminal, IMaterialModifier
{
    public event Action<GameObject> onEditMaterial;
    
    public override event Action onTerminalInteracted
    {
        add => onInteracted.AddListener(new UnityAction(value));
        remove => onInteracted.RemoveListener(new UnityAction(value));
    }
    
    [SerializeField] private UnityEvent onInteracted;
    [SerializeField] private bool automaticallyInteract;

    private PrecursorTeleporterActivationTerminal activationTerminal;
    private bool interacted;
    
    private void Start()
    {
        if (automaticallyInteract)
        {
            ForceInteracted();
        }
        
        UWE.CoroutineHost.StartCoroutine(RetrievePrefab());
    }

    private IEnumerator RetrievePrefab()
    {
        var prefabRequest = UWE.PrefabDatabase.GetPrefabAsync("2cd64262-7029-4dc2-8fa2-9cd0a025e8fe");
        yield return prefabRequest;
        
        if (!prefabRequest.TryGetPrefab(out var prefab)) throw new Exception("Error retrieving precursor ion cube receptacle prefab!");

        prefab.SetActive(false);
        SpawnPrefab(prefab);
    }
    
    private void SpawnPrefab(GameObject prefab)
    {
        var instance = Instantiate(prefab, transform);
        DestroyImmediate(instance.transform.Find("Precursor_Prison_Teleporter_ToCragField(Placeholder)").gameObject);
        
        instance.transform.SetParent(transform, false);
        instance.transform.localPosition = new Vector3(0, -0.5f, -5.75f);
        instance.transform.localRotation = Quaternion.identity;
        instance.transform.localScale = Vector3.one;
        instance.SetActive(true);

        activationTerminal = instance.GetComponent<PrecursorTeleporterActivationTerminal>();
        activationTerminal.cinematicController.informGameObject = gameObject;

        Destroy(instance.GetComponent<PrefabIdentifier>());
        Destroy(instance.GetComponent<LargeWorldEntity>());
        Destroy(instance.GetComponent<TechTag>());
        
        var applier = GetComponentInParent<SkyApplier>();
        if (applier)
        {
            applier.renderers.AddRangeToArray(GetComponentsInChildren<Renderer>(true));
            applier.ApplySkybox();
        }

        if (interacted)
        {
            activationTerminal.unlocked = true;
            onInteracted?.Invoke();
        }

        foreach (var rend in instance.GetComponentsInChildren<Renderer>(true))
        {
            onEditMaterial?.Invoke(rend.gameObject);
        }
    }

    public void OnPlayerCinematicModeEnd(PlayerCinematicController controller)
    {
        if (activationTerminal.crystalObject)
        {
            Destroy(activationTerminal.crystalObject);
        }

        activationTerminal.CloseDeck();
        if (activationTerminal.restoreQuickSlot != -1)
        {
            Inventory.main.quickSlots.Select(activationTerminal.restoreQuickSlot);
        }
        
        onInteracted?.Invoke();
    }

    public override void ForceInteracted()
    {
        interacted = true;
        if (activationTerminal)
        {
            activationTerminal.unlocked = true;
            onInteracted?.Invoke();
        }
    }
}