using PrototypeSubMod.SaveData;
using SubLibrary.SaveData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UWE;

namespace PrototypeSubMod.PowerSystem;

internal class ProtoPowerAbilitySystem : MonoBehaviour, ISaveDataListener, ILateSaveDataListener
{
    public static readonly string EquipmentLabel = "ProtoPowerExtractorLabel";
    public static readonly string SlotName = "ProtoPowerConsumptionSlot";

    public Equipment equipment { get; private set; }
    public event Action onEquip;
    public event Action onUnequip;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private Transform powerObjectHolder;
    [SerializeField] private float maxDistance;
    [SerializeField] private FMODAsset depotOpenSFX;
    [SerializeField] private FMODAsset depotCloseSFX;

    [Header("Equipment Setup")]
    [SerializeField] private ChildObjectIdentifier storageRoot;
    [SerializeField] private ChildObjectIdentifier functionalityRoot;
    [SerializeField] private FMODAsset equipSound;
    [SerializeField] private FMODAsset unequipSound;

    private bool justRemoved;
    private PowerSourceFunctionality currentPowerFunctionality;
    private AbilityConsumptionButton abilityConsumptionButton;

    private Dictionary<TechType, GameObject> powerSourceGameObjects = new();

    private void Awake()
    {
        Initialize();
    }

    private IEnumerator Start()
    {
        animator.SetBool("Activated", false);

        foreach (var availableSource in PrototypePowerSystem.AllowedPowerSources.Keys)
        {
            var prefab = CraftData.GetPrefabForTechTypeAsync(availableSource);
            yield return prefab;

            var powerPrefabObj = Instantiate(prefab.GetResult(), powerObjectHolder);
            powerPrefabObj.transform.localPosition = Vector3.zero;
            foreach (var col in powerPrefabObj.GetComponentsInChildren<Collider>(true))
            {
                Destroy(col);
            }

            foreach (var emitter in powerPrefabObj.GetComponentsInChildren<FMOD_CustomEmitter>(true))
            {
                Destroy(emitter);
            }

            foreach (var rigidBody in powerPrefabObj.GetComponentsInChildren<Rigidbody>(true))
            {
                Destroy(rigidBody);
            }

            Vector3 modelCenter = Vector3.zero;
            int rendCount = 0;
            foreach (var rend in powerPrefabObj.GetComponentsInChildren<Renderer>(true))
            {
                rendCount++;
                modelCenter += rend.bounds.center;
            }
            modelCenter /= rendCount;

            Vector3 delta = powerObjectHolder.InverseTransformPoint(powerObjectHolder.position) - powerObjectHolder.InverseTransformPoint(modelCenter);
            powerPrefabObj.transform.localPosition = delta;

            powerSourceGameObjects.Add(availableSource, powerPrefabObj);

            var itemInSlot = equipment.GetItemInSlot(SlotName);
            bool active = itemInSlot != null && itemInSlot.techType == availableSource;
            powerPrefabObj.SetActive(active);
        }
    }

    private void Initialize()
    {
        if (equipment != null) return;

        equipment = new(gameObject, storageRoot.transform);
        equipment.SetLabel(EquipmentLabel);
        equipment.onEquip += OnEquip;
        equipment.onUnequip += OnUnequip;

        equipment.AddSlots(new[] { SlotName });

        equipment.isAllowedToAdd = IsAllowedToAdd;
        equipment.isAllowedToRemove = (p, v) =>
        {
            return true;
        };

        abilityConsumptionButton = uGUI_PDA.main.transform.Find("Content/InventoryTab/Equipment/PowerAbilityConsumeButton(Clone)").GetComponent<AbilityConsumptionButton>();
    }

    public void OnHover(HandTargetEventData eventData)
    {
        if (Plugin.GlobalSaveData.prototypeDestroyed) return;

        HandReticle main = HandReticle.main;

        if (PowerSourceActive())
        {
            int minutesLeft = (int)(GetPowerAbilityTimeRemaining() / 60);
            int secondsLeft = Mathf.RoundToInt(GetPowerAbilityTimeRemaining() % (minutesLeft * 60));

            string message = Language.main.GetFormat("ProtoAbilityRemaining", minutesLeft, secondsLeft);

            main.SetText(HandReticle.TextType.Hand, "ProtoAbilitySystemActive", true, GameInput.Button.None);
            main.SetText(HandReticle.TextType.HandSubscript, message, false, GameInput.Button.None);
        }
        else
        {
            main.SetText(HandReticle.TextType.Hand, "UsePowerAbilitySystem", true, GameInput.Button.LeftHand);
            main.SetText(HandReticle.TextType.HandSubscript, string.Empty, false, GameInput.Button.None);
            main.SetIcon(HandReticle.IconType.Hand, 1f);
        }
    }

    public void OnUse(HandTargetEventData eventData)
    {
        if (Plugin.GlobalSaveData.prototypeDestroyed) return;

        if (PowerSourceActive()) return;

        PDA pda = Player.main.GetPDA();
        Inventory.main.SetUsedStorage(equipment);

        abilityConsumptionButton.SetActiveAbilitySystem(this);
        pda.Open(PDATab.Inventory, null, new PDA.OnClose(abilityConsumptionButton.OnClosePDA));
    }

    private void OnEquip(string slot, InventoryItem item)
    {
        if (equipSound != null)
        {
            FMODUWE.PlayOneShot(equipSound, transform.position);
        }
        
        onEquip?.Invoke();

        foreach (var obj in powerSourceGameObjects.Values)
        {
            obj.SetActive(false);
        }

        if (powerSourceGameObjects.TryGetValue(item.techType, out var powerObj))
        {
            powerObj.SetActive(true);
        }
    }

    private void OnUnequip(string slot, InventoryItem item)
    {
        onUnequip?.Invoke();

        if (justRemoved)
        {
            justRemoved = false;
            return;
        }

        if (unequipSound != null)
        {
            FMODUWE.PlayOneShot(unequipSound, transform.position);
        }
        
        foreach (var obj in powerSourceGameObjects.Values)
        {
            obj.SetActive(false);
        }
    }

    private bool IsAllowedToAdd(Pickupable pickupable, bool verbose)
    {
        return PrototypePowerSystem.AllowedPowerSources.Keys.Contains(pickupable.GetTechType());
    }

    public void ConsumeItem()
    {
        CoroutineHost.StartCoroutine(ConsumeItemAsync());
    }

    private IEnumerator ConsumeItemAsync()
    {
        var currentItem = equipment.GetItemInSlot(SlotName);
        var effectType = PrototypePowerSystem.AllowedPowerSources[currentItem.techType].SourceEffectFunctionality;
        if (effectType != null)
        {
            var component = functionalityRoot.gameObject.AddComponent(effectType);
            currentPowerFunctionality = component as PowerSourceFunctionality;
        }

        justRemoved = true;

        equipment.RemoveItem(SlotName, true, true);
        Destroy(currentItem.item.gameObject);

        PDA pda = Player.main.GetPDA();
        pda.Close();

        yield return new WaitForSeconds(0.75f);
        animator.SetBool("ProxyActivated", false);
        animator.SetBool("OnCooldown", true);

        yield return new WaitForSeconds(2f);
        foreach (var obj in powerSourceGameObjects.Values)
        {
            obj.SetActive(false);
        }
    }

    public bool HasItem()
    {
        return equipment.GetItemInSlot(SlotName) != null;
    }

    public void OnSaveDataLoaded(BaseSubDataClass saveData)
    {
        Initialize();
    }

    public void OnBeforeDataSaved(ref BaseSubDataClass saveData)
    {
        var protoData = saveData.EnsureAsPrototypeData();
        protoData.serializedPowerAbilityEquipment = equipment.SaveEquipment();

        saveData = protoData;
    }

    public void OnProtoSerializeObjectTree(ProtobufSerializer serializer) { }

    public void OnLateSaveDataLoaded(BaseSubDataClass saveData)
    {
        var data = saveData.EnsureAsPrototypeData();
        if (data.serializedPowerAbilityEquipment != null)
        {
            StorageHelper.TransferEquipment(storageRoot.gameObject, data.serializedPowerAbilityEquipment, equipment);
        }
    }

    public bool PowerSourceActive()
    {
        return currentPowerFunctionality != null;
    }

    public float GetPowerAbilityTimeRemaining()
    {
        return currentPowerFunctionality.GetTimeLeft();
    }

    public void CheckForCurrentFunctionality()
    {
        currentPowerFunctionality = functionalityRoot.GetComponent<PowerSourceFunctionality>();
        animator.SetBool("OnCooldown", currentPowerFunctionality != null);
    }

    public void OnEnterProxy()
    {
        if (currentPowerFunctionality != null || Plugin.GlobalSaveData.prototypeDestroyed) return;

        animator.SetBool("ProxyActivated", true);
        animator.SetBool("OnCooldown", false);

        FMODUWE.PlayOneShot(depotOpenSFX, transform.position, 0.35f);
    }

    public void OnExitProxy()
    {
        if (currentPowerFunctionality != null) return;

        animator.SetBool("ProxyActivated", false);
        animator.SetBool("OnCooldown", false);

        FMODUWE.PlayOneShot(depotCloseSFX, transform.position, 0.35f);
    }
}
