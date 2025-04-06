using System.Collections.Generic;
using UnityEngine;

namespace PrototypeSubMod.PowerSystem;

public class PowerDepositManager : MonoBehaviour, IItemSelectorManager
{
    [SerializeField] private PrototypePowerSystem powerSystem;
    
    public bool Filter(InventoryItem item)
    {
        return PrototypePowerSystem.AllowedPowerSources.ContainsKey(item.techType);
    }

    public int Sort(List<InventoryItem> items)
    {
        foreach (InventoryItem item in items)
        {
            Plugin.Logger.LogInfo(item);
        }
        
        // If there are no available items in the inventory
        if (items.Count == 0) return -1;
        
        return 0;
    }

    public string GetText(InventoryItem item)
    {
        if (item == null)
        {
            return Language.main.Get("ProtoCancelSelection");
        }
        
        return Language.main.Get(item.item.GetTechName());
    }

    public void Select(InventoryItem item)
    {
        ErrorMessage.AddError($"Selected {item}");
    }

    public void OpenSelection()
    {
        uGUI.main.itemSelector.Initialize(this, SpriteManager.Get(SpriteManager.Group.Item, "nobattery"), new List<IItemsContainer>
        {
            Inventory.main.container
        });
    }
}