using System;
using PrototypeSubMod.MiscMonobehaviors.SubSystems;
using PrototypeSubMod.PowerSystem;
using SubLibrary.UI;
using UnityEngine;

namespace PrototypeSubMod.UI.PowerDisplay;

public class ProtoChargeDisplay : MonoBehaviour, IUIElement
{
    [SerializeField] private PrototypePowerSystem powerSystem;
    [SerializeField] private OnModifyPowerEvent onModifyPower;
    [SerializeField] private GameObject chargeIconPrefab;
    [SerializeField] private Transform iconsParent;
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite lowPowerSprite;

    private int chargesLastCheck;
    
    private void Start()
    {
        onModifyPower.onModifyPower += _ => UpdateCharges();
        powerSystem.onReorderSources += RegenerateCharges;

        RegenerateCharges();
    }

    public void UpdateUI() { }

    public void OnSubDestroyed()
    {
        UpdateCharges();
    }

    private void RegenerateCharges()
    {
        foreach (Transform child in iconsParent)
        {
            Destroy(child.gameObject);
        }

        if (powerSystem.equipment.GetItemInSlot(PrototypePowerSystem.SLOT_NAMES[0]) == null) return;

        var currentSource = powerSystem.GetPowerSources()[0];
        for (int i = 0; i < currentSource.GetRemainingCharges(); i++)
        {
            var newIcon= Instantiate(chargeIconPrefab, iconsParent);
            newIcon.GetComponent<ProtoChargeIcon>().SetSprite(normalSprite);
        }
        
        chargesLastCheck = currentSource.GetRemainingCharges();
    }
    
    private void UpdateCharges()
    {
        var currentSource = powerSystem.GetPowerSources()[0];
        var chargeIcon = iconsParent.GetChild(0).GetComponent<ProtoChargeIcon>();
        if (currentSource.GetCurrentChargePower01() <= 0.25f)
        {
            chargeIcon.SetSprite(lowPowerSprite);
        }
        else
        {
            chargeIcon.SetSprite(normalSprite);
        }
        
        int remainingCharges = currentSource.GetRemainingCharges();
        if (chargesLastCheck != remainingCharges)
        {
            RegenerateCharges();
        }
        
        chargesLastCheck = remainingCharges;
    }
}