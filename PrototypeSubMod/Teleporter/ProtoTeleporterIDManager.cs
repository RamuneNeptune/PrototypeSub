using System.Collections;
using TMPro;
using UnityEngine;

namespace PrototypeSubMod.Teleporter;

internal class ProtoTeleporterIDManager : MonoBehaviour
{
    [SerializeField] private ProtoTeleporterManager positionSetter;
    [SerializeField] private GameObject teleporterLocationPrefab;
    [SerializeField] private Transform prefabSpawnParent;
    [SerializeField] private Animator animator;

    private AnimatorStateInfo targetState;
    private bool screenActive;

    private void Start()
    {
        RefreshLocationList();
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
        foreach (Transform child in prefabSpawnParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var item in TeleporterManager.main.activeTeleporters)
        {
            if (item.ToLower().Contains("proto") && item != "protoislandtp") continue;

            if (item != "protoislandtp")
            {
                var locationItemM = Instantiate(teleporterLocationPrefab, prefabSpawnParent).GetComponent<TeleporterLocationItem>();
                locationItemM.SetInfo(item, true, this);
            }
            
            var locationItemS = Instantiate(teleporterLocationPrefab, prefabSpawnParent).GetComponent<TeleporterLocationItem>();
            locationItemS.SetInfo(item, false, this);
        }
    }

    public void OnItemSelected(string id, bool isHost)
    {
        positionSetter.SetTeleporterID(id);
        positionSetter.SetTeleporterIsHost(isHost);

        string endLetter = isHost ? "M" : "S";
        string languageKey = $"{id}{endLetter}_ProtoLabel";
    }

    public void SetScreenActive(bool active)
    {
        screenActive = active;
        animator.SetBool("ScreenActive", active);
        targetState = animator.GetNextAnimatorStateInfo(0);

        StartCoroutine(UpdateText());
    }

    public void ToggleScreenActive()
    {
        SetScreenActive(!screenActive);
    }

    private IEnumerator UpdateText()
    {
        if (!prefabSpawnParent) yield break;
        
        while (!animator.GetCurrentAnimatorStateInfo(0).Equals(targetState))
        {
            foreach (var item in prefabSpawnParent.GetComponentsInChildren<TeleporterLocationItem>())
            {
                item.SetTextDirty();
            }

            yield return new WaitForSecondsRealtime(0.1f);
        }
    }
}
