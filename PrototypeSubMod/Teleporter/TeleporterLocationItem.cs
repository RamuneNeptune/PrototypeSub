using UnityEngine;
using UnityEngine.UI;

namespace PrototypeSubMod.Teleporter;

public class TeleporterLocationItem : MonoBehaviour
{
    [SerializeField] private string teleporterID;
    [SerializeField] private bool isHost;
    [SerializeField] private ProtoTeleporterIDManager idManager;
    [SerializeField] private Image image;
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite selectedSprite;

    private bool selected;
    
    public void SetInfo(string id, bool isHost, ProtoTeleporterIDManager manager)
    {
        teleporterID = id;
        this.isHost = isHost;
        idManager = manager;
    }

    public string GetTeleporterID()
    {
        return teleporterID;
    }

    public void OnButtonClicked()
    {
        idManager.OnItemSelected(teleporterID, isHost);
        SetSelected(true);
    }

    public void SetSelected(bool selected)
    {
        this.selected = selected;
        image.sprite = selected ? selectedSprite : normalSprite;
    }
}
