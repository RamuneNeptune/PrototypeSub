using PrototypeSubMod.MiscMonobehaviors.SubSystems;
using PrototypeSubMod.Upgrades;
using System.Collections;
using PrototypeSubMod.Facilities.Interceptor;
using UnityEngine;

namespace PrototypeSubMod.Teleporter;

internal class ProtoTeleporterManager : ProtoUpgrade
{
    internal static readonly Color TeleportScreenColInner = new Color(0.5638f, 0.4349f, 0.6674f, 0.4970f);
    internal static readonly Color TeleportScreenColMiddle = new Color(0.15f, 0.1905f, 1.0000f, 0.3000f);
    internal static readonly Color TeleportScreenColOuter = new Color(0.4412f, 0.4285f, 0.7118f, 0.4790f);
    
    [Header("Teleporting")]
    [SerializeField] private PrecursorTeleporter teleporter;
    [SerializeField] private TeleporterModeManager modeManager;
    [SerializeField] private SubRoot subRoot;
    [SerializeField] private Transform teleportPosition;
    [SerializeField] private FMOD_CustomLoopingEmitter activeLoopSound;
    [SerializeField] private VoiceNotification overrideStatus1;
    [SerializeField] private VoiceNotification overrideStatus2;
    [SerializeField] private string teleporterID;
    [SerializeField] private float stayOpenTime;

    private bool teleporterClosed = true;
    private float currentStayOpenTime;
    private PrecursorTeleporterActivationTerminal activationTerminal;

    private void OnValidate()
    {
        if (!teleporter) TryGetComponent(out teleporter);
    }

    private void Start()
    {
        PrecursorTeleporter.TeleportEventEnd += OnTeleportEnd;
        TeleporterManager.main.activeTeleporters.Remove("prototypetp");

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
    public void BeginTeleportPlayer(GameObject player)
    {
        TeleporterPositionHandler.TeleportData positionData = default;
        if (!TeleporterPositionHandler.TeleporterPositions.TryGetValue(teleporterID, out positionData))
        {
            throw new System.Exception($"Tried to teleport to unknown ID: \"{teleporterID}\". Position unknown");
        }

        teleporter.warpToPos = positionData.teleportPosition;
        teleporter.warpToAngle = positionData.teleportAngle;

        currentStayOpenTime = 0;

        TeleporterOverride.SetOverrideTeleporterID(teleporterID);
        TeleporterOverride.SetOverrideTime(120f);
        TeleporterOverride.OnTeleportStarted(this);

        Camera.main.GetComponent<ProtoScreenTeleporterFXManager>().SetColors(TeleportScreenColInner, TeleportScreenColMiddle, TeleportScreenColOuter);
        
        if (teleporterID == "protoislandtpS")
        {
            InterceptorIslandManager.Instance.OnTeleportToIsland(Vector3.zero);
            InterceptorIslandManager.Instance.GetComponentsInChildren<TeleporterOverride>().Initialize();
        }
    }

    public void SetTeleporterID(string id)
    {
        teleporterID = id;
    }

    private void OnTeleportEnd()
    {
        if (!TeleporterOverride.QueuedTeleportedBackToSub) return;

        TeleporterOverride.OnTeleportToSubFinished();
        modeManager.SetInterfloorMode();

        Player.main.SetCurrentSub(subRoot, true);
        
    }

    public Transform GetTeleportPosition() => teleportPosition;
    public string GetTeleporterID() => teleporterID;

    /// <summary>
    /// Returns the teleporter ID without the M/S indicator
    /// </summary>
    /// <returns></returns>
    public string GetTeleporterIDNoIndicator()
    {
        return teleporterID.Replace("M", string.Empty).Replace("S", string.Empty);
    }

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
        activationTerminal.GetComponentInChildren<Collider>(true).isTrigger = true;
        StartCoroutine(FallbackEnableCollider());
    }

    public void OnActivationTerminalCinematicEnded()
    {
        activationTerminal.GetComponentInChildren<Collider>(true).isTrigger = false;
    }

    private IEnumerator FallbackEnableCollider()
    {
        yield return new WaitForSeconds(7.5f);

        activationTerminal.GetComponentInChildren<Collider>(true).isTrigger = false;
    }

    public void PlayOverrideMarker1()
    {
        overrideStatus1.Play();
    }

    public void PlayOverrideMarker2()
    {
        overrideStatus2.Play();
    }

    public override bool GetUpgradeEnabled() => upgradeInstalled;
    
    public override bool OnActivated() => false;
    public override void OnSelectedChanged(bool selected) { }
}

public struct ColorOverrideData
{
    public bool overrideActive;
    public Color innerColor;
    public Color middleColor;
    public Color outerColor;

    public ColorOverrideData(bool overrideActive, Color innerColor, Color middleColor, Color outerColor)
    {
        this.overrideActive = overrideActive;
        this.innerColor = innerColor;
        this.middleColor = middleColor;
        this.outerColor = outerColor;
    }
}