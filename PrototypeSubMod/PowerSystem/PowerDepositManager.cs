using System.Collections.Generic;
using UnityEngine;

namespace PrototypeSubMod.PowerSystem;

public class PowerDepositManager : MonoBehaviour, IItemSelectorManager
{
    [SerializeField] private SubRoot subRoot;
    [SerializeField] private PrototypePowerSystem powerSystem;
    [SerializeField] private VoiceNotification powerLockedNotif;
    
    private bool storyLocked;
    
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
    
    public void OnHover(HandTargetEventData eventData)
    {
        HandReticle main = HandReticle.main;
        main.SetText(HandReticle.TextType.Hand, "UseProtoPowerSystem", true, GameInput.Button.LeftHand);
        main.SetText(HandReticle.TextType.HandSubscript, string.Empty, false);
        main.SetIcon(HandReticle.IconType.Hand, 1f);
    }

    public void OnUse(HandTargetEventData eventData)
    {
        if (storyLocked)
        {
            subRoot.voiceNotificationManager.PlayVoiceNotification(powerLockedNotif);
            return;
        }

        OpenSelection();
    }
    
    public void SetStoryLocked(bool locked)
    {
        storyLocked = locked;
    }
}