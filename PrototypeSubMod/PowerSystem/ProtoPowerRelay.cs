using System;
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
        var battery = inventoryItem.item.GetComponent<PrototypePowerBattery>();
        battery.ModifyCharge(-999999);
    }
}