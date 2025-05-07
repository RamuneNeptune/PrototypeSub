using TMPro;
using UnityEngine;

namespace PrototypeSubMod.Teleporter;

public class TeleporterLocationItem : MonoBehaviour
{
    [SerializeField] private string teleporterID;
    [SerializeField] private bool isHost;
    [SerializeField] private ProtoTeleporterIDManager idManager;

    public void SetInfo(string id, bool isHost, ProtoTeleporterIDManager manager)
    {
        teleporterID = id;
        this.isHost = isHost;
        idManager = manager;
    }

    public void OnButtonClicked()
    {
        idManager.OnItemSelected(teleporterID, isHost);
    }
}
