using PrototypeSubMod.Upgrades;
using System.Collections;
using PrototypeSubMod.PowerSystem;
using PrototypeSubMod.Utility;
using UnityEngine;

namespace PrototypeSubMod.Teleporter;

internal class ProtoEmergencyWarp : ProtoUpgrade
{
    [SaveStateReference]
    public static ProtoEmergencyWarp activeWarp;
    
    private Vector3 SUB_TELEPORT_POSITION { get; } = new Vector3(466.547f, -114.055f, 1219.048f);
    private const float TELEPORT_ANGLE = 20f;

    [SerializeField] private Rigidbody subRigidbody;
    [SerializeField] private SubRoot subRoot;
    [SerializeField] private VoiceNotification emergencyWarpNotification;
    [SerializeField] private VoiceNotification insufficientPowerNotification;
    [SerializeField] private PilotingChair pilotingChair;
    [SerializeField] private int requiredCharges;
    [SerializeField] private float chargeTime;
    [SerializeField] private FMOD_CustomEmitter emergencyWarpSFX;

    private float currentChargeTime = Mathf.Infinity;
    private bool startedTeleport = true;
    private bool teleportingToMoonpool;
    private bool isCharging;

    public void StartTeleportChargeUp()
    {
        if (!upgradeInstalled) return;

        subRoot.voiceNotificationManager.PlayVoiceNotification(emergencyWarpNotification);
        emergencyWarpSFX.Play();

        subRoot.powerRelay.ConsumeEnergy(999999, out _);
        currentChargeTime = 0;
        startedTeleport = false;
    }

    private void Update()
    {
        if (currentChargeTime < chargeTime)
        {
            currentChargeTime += Time.deltaTime;
            isCharging = true;
        }
        else if (currentChargeTime >= chargeTime && !startedTeleport)
        {
            TeleportToMoonpool();
            isCharging = false;
        }
    }

    private void OnTeleportationComplete()
    {
        if (!teleportingToMoonpool) return;

        subRigidbody.isKinematic = false;
        Player.main.GetComponent<Collider>().enabled = true;
        teleportingToMoonpool = false;

        UWE.CoroutineHost.StartCoroutine(ReEnterPilotingModeDelayed());
        activeWarp = null;
    }

    private void TeleportToMoonpool()
    {
        UWE.CoroutineHost.StartCoroutine(TeleportSub());
        startedTeleport = true;
    }

    private IEnumerator TeleportSub()
    {
        var player = Player.main;
        player.AddUsedTool(TechType.PrecursorTeleporter);

        player.cinematicModeActive = true;
        player.playerController.inputEnabled = false;
        Inventory.main.quickSlots.SetIgnoreHotkeyInput(true);
        player.GetPDA().Close();
        player.GetPDA().SetIgnorePDAInput(true);
        player.teleportingLoopSound.Play();
        player.GetComponent<Collider>().enabled = false;
        player.onTeleportationComplete += OnTeleportationComplete;

        Camera.main.GetComponent<TeleportScreenFXController>().StartTeleport();
        subRigidbody.isKinematic = true;
        subRigidbody.velocity = Vector3.zero;
        teleportingToMoonpool = true;

        Player.main.mode = Player.Mode.LockedPiloting;

        yield return new WaitForSeconds(1f);

        SetWarpPosition();
    }

    private void SetWarpPosition()
    {
        Quaternion quaternion = Quaternion.Euler(0, TELEPORT_ANGLE, 0);

        subRoot.transform.position = SUB_TELEPORT_POSITION;
        subRoot.transform.rotation = quaternion;

        Player.main.WaitForTeleportation();
    }

    private IEnumerator ReEnterPilotingModeDelayed()
    {
        yield return new WaitForEndOfFrame();
        
        Player.main.EnterPilotingMode(pilotingChair);
    }

    public override bool GetUpgradeEnabled() => upgradeEnabled;

    public override bool OnActivated()
    {
        if (subRoot.powerRelay.GetPower() < PrototypePowerSystem.CHARGE_POWER_AMOUNT * requiredCharges && GameModeUtils.RequiresPower())
        {
            subRoot.voiceNotificationManager.PlayVoiceNotification(insufficientPowerNotification);
            return false;
        }
        
        if (isCharging) return false;
        
        StartTeleportChargeUp();
        activeWarp = this;
        return true;
    }

    public bool IsCharging() => isCharging;

    public override void OnSelectedChanged(bool changed) { }
}
