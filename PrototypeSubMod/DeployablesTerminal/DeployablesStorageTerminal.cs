using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace PrototypeSubMod.DeployablesTerminal;

internal class DeployablesStorageTerminal : MonoBehaviour
{
    public static string[] SLOT_NAMES = new[]
    {
        "DeployableStorageSlot1",
        "DeployableStorageSlot2",
        "DeployableStorageSlot3"
    };

    public Equipment equipment { get; private set; }

    [SerializeField] private GameObject storageRoot;
    [SerializeField] private EquipmentType equipmentType;

    private void Awake()
    {
        SetupSlotMapping();
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
            { equipmentType, SLOT_NAMES.ToList() }
        };
    }

    private void SetupSlotMapping()
    {
        foreach (string name in SLOT_NAMES)
        {
            if (!Equipment.slotMapping.ContainsKey(name))
            {
                Equipment.slotMapping.Add(name, equipmentType);
            }
            else
            {
                Equipment.slotMapping[name] = equipmentType;
            }
        }
    }

    private void OnEquip(string slot, InventoryItem item)
    {

    }

    private void OnUnequip(string slot, InventoryItem item)
    {

    }

    public void SetEquipmentType(EquipmentType type)
    {
        equipmentType = type;

        equipment.typeToSlots = new Dictionary<EquipmentType, List<string>>()
        {
            { equipmentType, SLOT_NAMES.ToList() }
        };

        SetupSlotMapping();
    }
}
