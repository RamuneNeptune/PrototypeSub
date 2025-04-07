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
    [SerializeField] private Animator cinematicAnimator;
    [SerializeField] private Transform itemHolder;
    [SerializeField] private float cinematicLength = 5f;
    
    private GameObject powerSourceObject;
    private int restoreQuickSlot = -1;
    private bool storyLocked;
    private bool inAnimation;

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
        if (item == null) return;

        controller.StartCinematicMode(Player.main);
        inAnimation = true;
        restoreQuickSlot = Inventory.main.quickSlots.activeSlot;
        Inventory.main.ReturnHeld();

        reactorAnimator.SetTrigger(AcceptSource);
        cinematicAnimator.SetTrigger(AcceptSource);
        StartCoroutine(ExitCinematicModeDelayed());

        if (!Inventory.main.TryRemoveItem(item.item)) throw new System.Exception($"Could not remove {item.item} from iventory");
        
        powerSourceObject = item.item.gameObject;
        powerSourceObject.transform.SetParent(itemHolder);
        powerSourceObject.transform.localPosition = Vector3.zero;
        powerSourceObject.transform.localRotation = Quaternion.identity;
        powerSourceObject.SetActive(true);
        var col = powerSourceObject.GetComponentInChildren<Collider>();
        if (col)
        {
            col.enabled = false;
        }

        if (powerSourceObject.TryGetComponent(out Rigidbody rb))
        {
            UWE.Utils.SetIsKinematicAndUpdateInterpolation(rb, true);
        }
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
        if (inAnimation) return;

        string text = powerSystem.StorageSlotsFull() ? "ProtoPowerFull" : "UseProtoPowerSystem";
        var icon = powerSystem.StorageSlotsFull() ? GameInput.Button.None : GameInput.Button.LeftHand;
        
        HandReticle main = HandReticle.main;
        main.SetText(HandReticle.TextType.Hand, text, true, icon);
        main.SetText(HandReticle.TextType.HandSubscript, string.Empty, false);
        main.SetIcon(HandReticle.IconType.Hand, 1f);
    }

    public void OnUse(HandTargetEventData eventData)
    {
        if (inAnimation) return;
        
        if (powerSystem.StorageSlotsFull()) return;
        
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
        if (powerSystem.StorageSlotsFull()) return;
        
        reactorAnimator.SetBool(HatchOpen, inBounds);
    }

    private IEnumerator ExitCinematicModeDelayed()
    {
        yield return new WaitForSeconds(cinematicLength);
        controller.EndCinematicMode();
    }

    public void SetInAnimation(bool inAnimation)
    {
        this.inAnimation = inAnimation;
    }

    public void InstallCurrentPowerSource()
    {
        powerSourceObject.SetActive(false);
        string slot = "";
        foreach (var key in powerSystem.equipment.equipment.Keys)
        {
            if (!powerSystem.equipment.equipment.ContainsKey(key))
            {
                slot = key;
                break;
            }
        }
        
        var inventoryItem = powerSourceObject.GetComponent<Pickupable>().inventoryItem;
        powerSystem.equipment.AddItem(slot, inventoryItem);
    }
}