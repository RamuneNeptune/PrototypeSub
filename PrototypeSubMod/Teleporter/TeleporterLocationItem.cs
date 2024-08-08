using TMPro;
using UnityEngine;

namespace PrototypeSubMod.Teleporter;

internal class TeleporterLocationItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemNameText;

    private string teleporterID;
    private bool isHost;
    private ProtoTeleporterIDManager idManager;

    public void SetInfo(string id, bool isHost, ProtoTeleporterIDManager manager)
    {
        teleporterID = id;
        this.isHost = isHost;
        idManager = manager;

        string endLetter = isHost ? "M" : "S";
        string languageKey = $"{id}{endLetter}_ProtoLabel";
        itemNameText.text = Language.main.Get(languageKey);
    }

    public void OnButtonClicked()
    {
        idManager.OnItemSelected(teleporterID, isHost);
    }
}
