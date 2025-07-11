using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PrototypeSubMod.VehicleAccess;

public class ProtoVehicleAccessTerminal : MonoBehaviour
{
    public const string SLOT_NAME = "VehicleAccessPlaceholderSlot";
    public const string EQUIPMENT_LABEL = "VehicleAccessEquipment";
    
    public Equipment equipment { get; private set; }

    [SerializeField] private VehicleDockingBay dockingBay;
    
    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (equipment != null) return;
        
        equipment = new(gameObject, transform);
        equipment.SetLabel(EQUIPMENT_LABEL);
        equipment.AddSlots(new[] { SLOT_NAME });

        equipment.typeToSlots = new Dictionary<EquipmentType, List<string>> { { EquipmentType.None, Array.Empty<string>().ToList() } };
    }
    
    public void OnHover(HandTargetEventData eventData)
    {
        string key = dockingBay.dockedVehicle ? "ProtoAccessVehicle" : "ProtoNoVehicleDocked";
        var main = HandReticle.main;
        main.SetText(HandReticle.TextType.Hand, key, true, GameInput.Button.LeftHand);
        main.SetText(HandReticle.TextType.HandSubscript, string.Empty, false);
        main.SetIcon(HandReticle.IconType.Hand, 1f);
    }

    public void OnUse(HandTargetEventData eventData)
    {
        if (!dockingBay.dockedVehicle) return;

        Inventory.main.SetUsedStorage(equipment);
        Player.main.pda.Open(PDATab.Inventory);
    }
}