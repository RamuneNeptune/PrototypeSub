using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PrototypeSubMod.PowerSystem;

public class PowerDepositManager : MonoBehaviour, IItemSelectorManager
{
    private static readonly int HatchOpen = Animator.StringToHash("HatchOpen");
    private static readonly int AcceptSource = Animator.StringToHash("AcceptSource");
    [SerializeField] private SubRoot subRoot;
    [SerializeField] private PrototypePowerSystem powerSystem;
    [SerializeField] private VoiceNotification powerLockedNotif;
    [SerializeField] private PlayerCinematicController controller;
    [SerializeField] private Animator reactorAnimator;
    [SerializeField] private float cinematicLength = 5f;
    
    private bool storyLocked;
    private int restoreQuickSlot = -1;

    private void Start()
    {
        controller.animator = Player.main.playerAnimator;
    }

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

        if (item == null) return;

        controller.StartCinematicMode(Player.main);
        restoreQuickSlot = Inventory.main.quickSlots.activeSlot;
        Inventory.main.ReturnHeld();

        reactorAnimator.SetTrigger(AcceptSource);
        StartCoroutine(ExitCinematicModeDelayed());
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

    public void OnPlayerCinematicModeEnd(PlayerCinematicController controller)
    {
        Inventory.main.quickSlots.Select(restoreQuickSlot);
    }

    public void OnPlayerProxyChanged(bool inBounds)
    {
        reactorAnimator.SetBool(HatchOpen, true);
    }

    private IEnumerator ExitCinematicModeDelayed()
    {
        yield return new WaitForSeconds(cinematicLength);
        controller.EndCinematicMode();
    }
}