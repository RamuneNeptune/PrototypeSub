using PrototypeSubMod.MiscMonobehaviors.SubSystems;
using PrototypeSubMod.Upgrades;
using System.Collections;
using UnityEngine;

namespace PrototypeSubMod.Teleporter;

internal class ProtoTeleporterManager : ProtoUpgrade
{
    public static ProtoTeleporterManager Instance { get; private set; }

    [Header("Teleporting")]
    [SerializeField] private PrecursorTeleporter teleporter;
    [SerializeField] private SubRoot subRoot;
    [SerializeField] private Transform teleportPosition;
    [SerializeField] private FMOD_CustomLoopingEmitter activeLoopSound;
    [SerializeField] private string teleporterID;
    [SerializeField] private bool isHost;
    [SerializeField] private float stayOpenTime;

    [Header("Power Cost")]
    [SerializeField] private float costPerMeter = 0.6f;
    [SerializeField] private float minPowercost = 400;
    [SerializeField] private float maxPowerCost = 1200;

    private bool teleporterClosed = true;
    private float currentStayOpenTime;
    private float powerCostMultiplier = 1f;
    private PrecursorTeleporterActivationTerminal activationTerminal;

    private ColorOverrideData colorOverrideData;

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
    public void BeginTeleportPlayer(GameObject player)
    {
        string alteredID = teleporterID + (isHost ? "M" : "S");
        var positionData = TeleporterPositionHandler.TeleporterPositions[alteredID];
        teleporter.warpToPos = positionData.teleportPosition;
        teleporter.warpToAngle = positionData.teleportAngle;

        currentStayOpenTime = 0;

        if (upgradeInstalled)
        {
            TeleporterOverride.SetOverrideTeleporterID(alteredID);
            TeleporterOverride.SetOverrideTime(120f);
            TeleporterOverride.OnTeleportStarted();
        }

        if (colorOverrideData.overrideActive)
        {
            Camera.main.GetComponent<ProtoScreenTeleporterFXManager>().SetColors(colorOverrideData.innerColor, colorOverrideData.middleColor, colorOverrideData.outerColor);
        }

        float energyCost = Vector3.Distance(positionData.teleportPosition, transform.position) * costPerMeter * powerCostMultiplier;
        energyCost = Mathf.Clamp(energyCost, minPowercost, maxPowerCost);
        subRoot.powerRelay.ConsumeEnergy(energyCost, out _);
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
        activationTerminal.OpenDeck();

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

    public void SetColorOverrideData(ColorOverrideData overrideData) => colorOverrideData = overrideData;
    public void ResetOverrideData() => colorOverrideData = default;

    public override bool GetUpgradeEnabled() => upgradeInstalled;

    public void SetPowerMultiplier(float multiplier) => powerCostMultiplier = multiplier;
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