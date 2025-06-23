using UnityEngine;

namespace PrototypeSubMod.Facilities.Defense;

public class OnEnableTeleporterInitializer : MonoBehaviour
{
    [SerializeField] private PrecursorTeleporter teleporter;

    private void OnEnable()
    {
        bool teleporterActive = TeleporterManager.GetTeleporterActive(teleporter.teleporterIdentifier);
        teleporter.InitializeDoor(teleporterActive);
    }
}