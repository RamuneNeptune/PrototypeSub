using System.Collections;
using TMPro;
using UnityEngine;

namespace PrototypeSubMod.Teleporter;

internal class ProtoTeleporterIDManager : MonoBehaviour
{
    [SerializeField] private TeleporterPositionSetter positionSetter;
    [SerializeField] private TextMeshProUGUI selectedLocationText;
    [SerializeField] private GameObject teleporterLocationPrefab;
    [SerializeField] private Transform prefabSpawnParent;
    [SerializeField] private Animator animator;

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
            if (item.ToLower().Contains("proto")) continue;

            var locationItemM = Instantiate(teleporterLocationPrefab, prefabSpawnParent).GetComponent<TeleporterLocationItem>();
            locationItemM.SetInfo(item, true, this);  

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
        selectedLocationText.text = Language.main.Get(languageKey);
    }

    private void OnTriggerEnter(Collider col)
    {
        if (!col.gameObject.Equals(Player.main.gameObject)) return;

        animator.SetBool("ScreenActive", true);
    }

    private void OnTriggerExit(Collider col)
    {
        if (!col.gameObject.Equals(Player.main.gameObject)) return;

        animator.SetBool("ScreenActive", false);
    }
}
