using System;
using System.Collections;
using System.Collections.Generic;
using PrototypeSubMod.MiscMonobehaviors.SubSystems;
using PrototypeSubMod.PowerSystem;
using UnityEngine;
using UnityEngine.UI;

namespace PrototypeSubMod.UI.PowerDisplay;

public class ProtoSourceDisplay : MonoBehaviour
{
    [SerializeField] private PrototypePowerSystem powerSystem;
    [SerializeField] private ProtoPowerIconManager iconManager;
    [SerializeField] private GameObject powerIconPrefab;
    [SerializeField] private Transform[] iconPositions;
    [SerializeField] private Image mainIcon;

    private readonly List<ProtoChargeIcon> activeSourceIcons = new();
    private int sourcesLastCheck;
    
    private void Start()
    {
        powerSystem.equipment.onAddItem += _ => RegenerateSources();
        powerSystem.onReorderSources += RegenerateSources;
        RegenerateSources();
    }

    private void RegenerateSources()
    {
        foreach (var icon in activeSourceIcons)
        {
            Destroy(icon.gameObject);
        }

        activeSourceIcons.Clear();
        
        int index = 0;
        foreach (var iconPos in iconPositions)
        {
            if (index >= powerSystem.GetInstalledSourceCount()) break;
            
            var newIcon = Instantiate(powerIconPrefab, iconPos);
            newIcon.transform.localPosition = Vector3.zero;
            var chargeIcon = newIcon.GetComponent<ProtoChargeIcon>();
            var item = powerSystem.equipment.GetItemInSlot(PrototypePowerSystem.SLOT_NAMES[index]);
            
            chargeIcon.SetSprite(iconManager.GetSpriteForTechType(item.techType));
            activeSourceIcons.Add(chargeIcon);
            index++;
        }

        sourcesLastCheck = powerSystem.GetInstalledSourceCount();
        if (sourcesLastCheck == 0)
        {
            mainIcon.gameObject.SetActive(false);
            return;
        }
        
        mainIcon.gameObject.SetActive(true);
        var itemAt0 = powerSystem.equipment.GetItemInSlot(PrototypePowerSystem.SLOT_NAMES[0]);
        mainIcon.sprite = iconManager.GetSpriteForTechType(itemAt0.techType);
    }
}