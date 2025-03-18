using PrototypeSubMod.SaveData;
using SubLibrary.SaveData;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PrototypeSubMod.DeployablesTerminal;

internal class DeployablesStorageTerminal : MonoBehaviour, ISaveDataListener, ILateSaveDataListener
{
    public static string[] SLOT_NAMES { get; } = new[]
    {
        "DeployableStorageSlot1",
        "DeployableStorageSlot2",
        "DeployableStorageSlot3",
        "DeployableStorageSlot4"
    };

    public static Vector3[] SLOT_POSITIONS { get; } = new[]
    {
        new Vector3(-166, -98, 0),
        new Vector3(28, 130, 0),
        new Vector3(-56, -212, 0),
        new Vector3(138.5f, 17.5f, 0),
    };

    public static string[] LightBeaconSlots { get; } = new[]
    {
        "DeployableStorageSlot1",
        "DeployableStorageSlot3",
    };

    public static string[] CreatureDecoySlots { get; } = new[]
    {
        "DeployableStorageSlot2",
        "DeployableStorageSlot4",
    };

    private static bool SlotmappingInitialized;

    public Equipment equipment { get; private set; }

    [SerializeField] private GameObject storageRoot;
    [SerializeField] private FMODAsset equipSound;
    [SerializeField] private FMODAsset unequipSound;
    [SerializeField] private ProtoDeployableManager deployableManager;

    private bool ignoreSoundNextEquip;

    private void Awake()
    {
        Initialize();
    }

    public void OnHover(HandTargetEventData eventData)
    {
        HandReticle main = HandReticle.main;
        main.SetText(HandReticle.TextType.Hand, "UseDeployableTerminal", true, GameInput.Button.LeftHand);
        main.SetText(HandReticle.TextType.HandSubscript, string.Empty, false, GameInput.Button.None);
        main.SetIcon(HandReticle.IconType.Hand, 1f);
    }

    public void OnUse(HandTargetEventData eventData)
    {
        PDA pda = Player.main.GetPDA();
        Inventory.main.SetUsedStorage(equipment);
        pda.Open(PDATab.Inventory);
    }

    private void Initialize()
    {
        if (equipment != null) return;

        InitializeSlotMapping();

        equipment = new(gameObject, storageRoot.transform);
        equipment.SetLabel("ProtoDeployableEquipmentLabel");
        equipment.onEquip += OnEquip;
        equipment.onUnequip += OnUnequip;

        equipment.AddSlots(SLOT_NAMES);

        equipment.typeToSlots = new Dictionary<EquipmentType, List<string>>()
        {
            { EquipmentType.DecoySlot, CreatureDecoySlots.ToList()},
            { Plugin.LightBeaconEquipmentType, LightBeaconSlots.ToList() }
        };
    }

    private void InitializeSlotMapping()
    {
        if (SlotmappingInitialized) return;

        foreach (string slot in LightBeaconSlots)
        {
            Equipment.slotMapping.Add(slot, Plugin.LightBeaconEquipmentType);
        }

        foreach (string slot in CreatureDecoySlots)
        {
            Equipment.slotMapping.Add(slot, EquipmentType.DecoySlot);
        }

        SlotmappingInitialized = true;
    }

    private void OnEquip(string slot, InventoryItem item)
    {
        if (equipSound != null && !ignoreSoundNextEquip)
        {
            FMODUWE.PlayOneShot(equipSound, transform.position, 2f);
        }

        deployableManager.RecalculateDeployableTotals();
        ignoreSoundNextEquip = false;
    }

    private void OnUnequip(string slot, InventoryItem item)
    {
        if (unequipSound != null)
        {
            FMODUWE.PlayOneShot(unequipSound, transform.position, 2f);
        }

        deployableManager.RecalculateDeployableTotals();
    }

    public void OnSaveDataLoaded(BaseSubDataClass saveData)
    {
        Initialize();
    }

    public void OnBeforeDataSaved(ref BaseSubDataClass saveData)
    {
        var protoData = saveData.EnsureAsPrototypeData();
        protoData.serializedDeployablesEquipment = equipment.SaveEquipment();

        saveData = protoData;
    }

    public void OnLateSaveDataLoaded(BaseSubDataClass saveData)
    {
        var data = saveData.EnsureAsPrototypeData();
        if (data.serializedDeployablesEquipment != null)
        {
            StorageHelper.TransferEquipment(storageRoot.gameObject, data.serializedDeployablesEquipment, equipment);
        }
    }

    public void IgnoreSoundNextEquip()
    {
        ignoreSoundNextEquip = true;
    }
}
