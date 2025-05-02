using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PrototypeSubMod.PowerSystem;

public class ProtoPowerRelay : MonoBehaviour
{
    private static readonly int PylonActive = Animator.StringToHash("PylonActive");
    
    [SerializeField] private Animator animator;
    [SerializeField] private ProtoPowerIconManager iconManager;
    [SerializeField] private GameObject iconCanvas;
    [SerializeField] private Image icon;
    
    private InventoryItem inventoryItem;
    private PrototypePowerSystem powerSystem;

    private void Start()
    {
        powerSystem = iconManager.GetComponent<PrototypePowerSystem>();
        if (inventoryItem != null)
        {
            icon.sprite = iconManager.GetSpriteForTechType(inventoryItem.techType);
        }
    }

    public void SetRelayActive(bool active)
    {
        animator.SetBool(PylonActive, active);
        iconCanvas.SetActive(active);
    }
    
    public bool GetRelayActive() => iconCanvas.activeSelf;

    public void SetPowerSource(InventoryItem item)
    {
        if (item != null)
        {
            icon.sprite = iconManager.GetSpriteForTechType(item.techType);
        }
        
        inventoryItem = item;
    }

    public void UninstallSource()
    {
        if (inventoryItem == null) return;
        
        if (!Inventory.main.HasRoomFor(inventoryItem.techType))
        {
            ErrorMessage.AddError(Language.main.Get("InventoryFull"));
            return;
        }
        
        powerSystem.equipment.RemoveItem(inventoryItem.item);
        Inventory.main.container.AddItem(inventoryItem.item);
        uGUI_IconNotifier.main.Play(inventoryItem.techType, uGUI_IconNotifier.AnimationType.From);
        inventoryItem.item.Pickup();
    }
}