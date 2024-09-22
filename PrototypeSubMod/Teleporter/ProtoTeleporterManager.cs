using PrototypeSubMod.Upgrades;
using System.Collections;
using UnityEngine;

namespace PrototypeSubMod.Teleporter;

internal class ProtoTeleporterManager : ProtoUpgrade
{
    public static ProtoTeleporterManager Instance { get; private set; }

    [SerializeField] private PrecursorTeleporter teleporter;
    [SerializeField] private SubRoot subRoot;
    [SerializeField] private Transform teleportPosition;
    [SerializeField] private FMOD_CustomLoopingEmitter activeLoopSound;
    [SerializeField] private string teleporterID;
    [SerializeField] private bool isHost;
    [SerializeField] private float stayOpenTime;

    private bool overrideUpgradeInstalled;
    private bool teleporterClosed = true;
    private float currentStayOpenTime;
    private PrecursorTeleporterActivationTerminal activationTerminal;

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

    private IEnumerator Start()
    {
        PrecursorTeleporter.TeleportEventEnd += OnTeleportEnd;
        TeleporterManager.main.activeTeleporters.Remove("prototypetp");

        for (int i = 0; i < 2; i++)
        {
            yield return new WaitForEndOfFrame();
        }

        activeLoopSound.Stop();
    }

    private void Update()
    {
        if (currentStayOpenTime > 0)
        {
            currentStayOpenTime -= Time.deltaTime;
        }
        else if (!teleporterClosed)
        {
            teleporter.ToggleDoor(false);
            ToggleDoor(false);
            teleporterClosed = true;
            activationTerminal.unlocked = false;

            DeactivateTeleporter();
        }
    }

    // Called by PrecursorTeleporterCollider.OnTriggerEnter via SendMessageUpwards
    public void BeginTeleportPlayer(GameObject _)
    {
        string alteredID = teleporterID + (isHost ? "M" : "S");
        var positionData = TeleporterPositionHandler.TeleporterPositions[alteredID];
        teleporter.warpToPos = positionData.teleportPosition;
        teleporter.warpToAngle = positionData.teleportAngle;

        currentStayOpenTime = 0;

        if (overrideUpgradeInstalled)
        {
            TeleporterOverride.SetOverrideTeleporterID(alteredID);
            TeleporterOverride.SetOverrideTime(120f);
            TeleporterOverride.OnTeleportStarted();
        }
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

    public override void SetUpgradeInstalled(bool active)
    {
        overrideUpgradeInstalled = active;
    }

    public override bool GetUpgradeInstalled() => overrideUpgradeInstalled;

    //Called by PrecursorTeleporterActivationTerminal via SendMessage
    public void ToggleDoor(bool open)
    {
        if (open)
        {
            teleporterClosed = false;
            currentStayOpenTime = stayOpenTime;
        }
        else
        {
            activeLoopSound.Stop();
            teleporter.isOpen = false;
        }

        activationTerminal = GetComponentInChildren<PrecursorTeleporterActivationTerminal>();
    }

    private void DeactivateTeleporter()
    {
        TeleporterManager.main.activeTeleporters.Remove("prototypetp");
    }

    public void OnActivationTerminalCinematicStarted()
    {
        activationTerminal = GetComponentInChildren<PrecursorTeleporterActivationTerminal>();

        activationTerminal.GetComponentInChildren<Collider>().isTrigger = true;
    }

    public void OnActivationTerminalCinematicEnded()
    {
        activationTerminal.GetComponentInChildren<Collider>().isTrigger = false;
    }

    public override string GetUpgradeName()
    {
        return "Archway Override";
    }

    public override void SetUpgradeEnabled(bool enabled)
    {
        // Not needed for this upgrade
    }

    public override bool GetUpgradeEnabled() => overrideUpgradeInstalled;
}
