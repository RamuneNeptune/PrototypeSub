using UnityEngine;

namespace PrototypeSubMod.Teleporter;

internal class TeleporterPositionSetter : MonoBehaviour
{
    public static TeleporterPositionSetter Instance { get; private set; }

    [SerializeField] private PrecursorTeleporter teleporter;
    [SerializeField] private SubRoot subRoot;
    [SerializeField] private Transform teleportPosition;
    [SerializeField] private string teleporterID;
    [SerializeField] private bool isHost;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    private void OnValidate()
    {
        if (!teleporter) TryGetComponent(out teleporter);
    }

    private void Start()
    {
        PrecursorTeleporter.TeleportEventEnd += OnTeleportEnd;
    }

    // Called by PrecursorTeleporterCollider.OnTriggerEnter via SendMessageUpwards
    public void BeginTeleportPlayer(GameObject _)
    {
        string alteredID = teleporterID + (isHost ? "M" : "S");
        var positionData = TeleporterPositionHandler.TeleporterPositions[alteredID];
        teleporter.warpToPos = positionData.teleportPosition;
        teleporter.warpToAngle = positionData.teleportAngle;

        TeleporterOverride.SetOverrideTeleporterID(alteredID);
        TeleporterOverride.SetOverrideTime(120f);
        TeleporterOverride.OnTeleportStarted();
    }

    public void SetTeleporterID(string id)
    {
        teleporterID = id;
    }

    public void SetTeleporterIsHost(bool isHost)
    {
        this.isHost = isHost;
    }

    private void OnTeleportEnd()
    {
        if (!TeleporterOverride.QueuedTeleportedBackToSub) return;

        TeleporterOverride.OnTeleportToSubFinished();

        Player.main.SetCurrentSub(subRoot, true);
    }

    public Transform GetTeleportPosition() => teleportPosition;
    public string GetTeleporterID() => teleporterID;
}
