using System.Collections.Generic;
using UnityEngine;

namespace PrototypeSubMod.DeployablesTerminal;

internal class DeployablesStorageTerminal : MonoBehaviour
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
        new Vector3(-152, 102, 0),
        new Vector3(135, 102, 0),
        new Vector3(-152, -113, 0),
        new Vector3(135, -113, 0),
    };

    public static List<string> LightBeaconSlots { get; } = new()
    {
        "DeployableStorageSlot1",
        "DeployableStorageSlot3",
    };

    public static List<string> CreatureDecoySlots { get; } = new()
    {
        "DeployableStorageSlot2",
        "DeployableStorageSlot4",
    };

    private static bool SlotmappingInitialized;

    public Equipment equipment { get; private set; }

    [SerializeField] private GameObject storageRoot;
    [SerializeField] private FMODAsset equipSound;
    [SerializeField] private FMODAsset unequipSound;

    private void Awake()
    {
        InitializeSlotMapping();
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

        equipment = new(gameObject, storageRoot.transform);
        equipment.SetLabel("ProtoDeployableEquipmentLabel");
        equipment.onEquip += OnEquip;
        equipment.onUnequip += OnUnequip;

        equipment.AddSlots(SLOT_NAMES);

        equipment.typeToSlots = new Dictionary<EquipmentType, List<string>>()
        {
            { EquipmentType.DecoySlot, CreatureDecoySlots},
            { Plugin.LightBeaconEquipmentType, LightBeaconSlots }
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
        if (equipSound != null)
        {
            FMODUWE.PlayOneShot(equipSound, transform.position, 2f);
        }
    }

    private void OnUnequip(string slot, InventoryItem item)
    {
        if (unequipSound != null)
        {
            FMODUWE.PlayOneShot(unequipSound, transform.position, 2f);
        }
    }
}
