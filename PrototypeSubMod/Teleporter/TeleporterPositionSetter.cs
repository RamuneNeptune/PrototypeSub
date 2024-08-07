using UnityEngine;

namespace PrototypeSubMod.Teleporter;

internal class TeleporterPositionSetter : MonoBehaviour
{
    [SerializeField] private PrecursorTeleporter teleporter;
    [SerializeField] private string teleporterID;
    [SerializeField] private bool isMaster;

    private void OnValidate()
    {
        if (!teleporter) TryGetComponent(out teleporter);
    }

    // Called by PrecursorTeleporterCollider.OnTriggerEnter via SendMessageUpwards
    public void BeginTeleportPlayer(GameObject _)
    {
        string alteredID = teleporterID + (isMaster ? "M" : "S");
        var positionData = TeleporterPositionHandler.TeleporterPositions[alteredID];
        teleporter.warpToPos = positionData.teleportPosition;
        teleporter.warpToAngle = positionData.teleportAngle;
    }

    public string GetTeleporterID() => teleporterID;
}
