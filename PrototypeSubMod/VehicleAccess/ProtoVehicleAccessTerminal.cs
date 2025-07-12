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

    private ProtoVehicleAccessManager accessManager;
    private uGUI_InventoryTab inventoryTab;
    
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

        accessManager = uGUI_PDA.main.GetComponentInChildren<ProtoVehicleAccessManager>(true);
        inventoryTab = uGUI_PDA.main.GetComponentInChildren<uGUI_InventoryTab>(true);
        accessManager.SetInventoryTab(inventoryTab);
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

        inventoryTab.usedStorageGrids.Clear();
        Inventory.main.SetUsedStorage(equipment);
        Player.main.pda.Open(PDATab.Inventory);
        inventoryTab.usedStorageGrids.Insert(0, accessManager);

        accessManager.SetTerminal(this);
    }

    public void OpenStorage(PDA.OnClose onClosePda = null)
    {
        var pda = Player.main.pda;
        pda.ui.OnClosePDA();
        Inventory.main.ClearUsedStorage();
        pda.onCloseCallback = null;
        
        foreach (var storageInput in dockingBay.dockedVehicle.GetComponentsInChildren<SeamothStorageInput>())
        {
            var storageInSlot = storageInput.seamoth.GetStorageInSlot(storageInput.slotID, TechType.VehicleStorageModule);
            Inventory.main.SetUsedStorage(storageInSlot, true);
        }
        
        foreach (var storageContainer in dockingBay.dockedVehicle.GetComponentsInChildren<StorageContainer>())
        {
            Inventory.main.SetUsedStorage(storageContainer.container, true);
            pda.onCloseCallback += storageContainer.OnClosePDA;
        }
        
        if (onClosePda != null)
        {
            pda.onCloseCallback += onClosePda;
        }
        pda.ui.OnOpenPDA(PDATab.Inventory);
        pda.ui.Select();
        pda.ui.OnPDAOpened();
    }

    public void OpenUpgrades(PDA.OnClose onClosePda = null)
    {
        var upgradeConsoleInput = dockingBay.dockedVehicle.GetComponentInChildren<VehicleUpgradeConsoleInput>();
        var pda = Player.main.pda;
        pda.ui.OnClosePDA();
        pda.onCloseCallback = null;
        
        Inventory.main.SetUsedStorage(upgradeConsoleInput.equipment);
        pda.ui.OnOpenPDA(PDATab.Inventory);
        pda.ui.Select();
        pda.ui.OnPDAOpened();
        
        if (onClosePda != null)
        {
            pda.onCloseCallback += onClosePda;
        }
    }

    public void QuickOpenManager(PDA.OnClose onClosePda = null)
    {
        var pda = Player.main.pda;
        pda.ui.OnClosePDA();
        pda.onCloseCallback = null;
        
        inventoryTab.usedStorageGrids.Clear();
        Inventory.main.SetUsedStorage(equipment);

        accessManager.SetTerminal(this);

        if (onClosePda != null)
        {
            pda.onCloseCallback += onClosePda;
        }
        pda.ui.OnOpenPDA(PDATab.Inventory);
        pda.ui.Select();
        pda.ui.OnPDAOpened();
        
        inventoryTab.usedStorageGrids.Insert(0, accessManager);
    }
}