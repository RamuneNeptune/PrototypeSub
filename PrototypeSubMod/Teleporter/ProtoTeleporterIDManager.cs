using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace PrototypeSubMod.Teleporter;

public class ProtoTeleporterIDManager : MonoBehaviour
{
    private static readonly int ScreenActive = Animator.StringToHash("ScreenActive");
    
    [SerializeField] private ProtoTeleporterManager positionSetter;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform surfaceTeleportersParent;
    [SerializeField] private Transform depthsTeleportersParent;

    private readonly Dictionary<string, TeleporterLocationItem> locationItems = new();
    private bool screenActive;
    
    private void Start()
    {
        PopulateLocationList();
        RefreshLocationList();
    }

    private void PopulateLocationList()
    {
        foreach (var locationItem in surfaceTeleportersParent.GetComponentsInChildren<TeleporterLocationItem>())
        {
            locationItems.Add(locationItem.GetTeleporterID(), locationItem);
            locationItem.gameObject.SetActive(false);
        }
        
        foreach (var locationItem in depthsTeleportersParent.GetComponentsInChildren<TeleporterLocationItem>())
        {
            locationItems.Add(locationItem.GetTeleporterID(), locationItem);
            locationItem.gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        TeleporterManager.TeleporterActivateEvent += OnTeleporterActivated;
    }

    private void OnDisable()
    {
        TeleporterManager.TeleporterActivateEvent -= OnTeleporterActivated;
    }

    private void OnTeleporterActivated(string identifier)
    {
        RefreshLocationList();
    }

    private void RefreshLocationList()
    {
        foreach (var item in TeleporterManager.main.activeTeleporters)
        {
            if (item.ToLower().Contains("proto") && item != "protoislandtp") continue;

            string keySource = item + "M";
            string keyTarget = item + "S";
            if (locationItems.TryGetValue(keySource, out TeleporterLocationItem locationItemM))
            {
                locationItemM.gameObject.SetActive(true);
            }
            
            if (locationItems.TryGetValue(keyTarget, out TeleporterLocationItem locationItemS))
            {
                locationItemS.gameObject.SetActive(true);
            }
        }
    }

    public void OnItemSelected(string id, bool isHost)
    {
        positionSetter.SetTeleporterID(id);
        foreach (var locationItem in locationItems)
        {
            if (locationItem.Key == id) continue;

            locationItem.Value.SetSelected(false);
        }
    }

    public void ToggleScreenActive()
    {
        screenActive = !screenActive;
        animator.SetBool(ScreenActive, screenActive);
    }
}
