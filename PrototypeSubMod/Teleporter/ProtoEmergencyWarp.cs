using PrototypeSubMod.Upgrades;
using System.Collections;
using PrototypeSubMod.PowerSystem;
using UnityEngine;

namespace PrototypeSubMod.Teleporter;

internal class ProtoEmergencyWarp : ProtoUpgrade
{
    public static bool isCharging;

    private Vector3 SUB_TELEPORT_POSITION { get; } = new Vector3(466.547f, -114.055f, 1219.048f);
    private const float TELEPORT_ANGLE = 20f;

    [SerializeField] private Rigidbody subRigidbody;
    [SerializeField] private SubRoot subRoot;
    [SerializeField] private VoiceNotification emergencyWarpNotification;
    [SerializeField] private VoiceNotification insufficientPowerNotification;
    [SerializeField] private PilotingChair pilotingChair;
    [SerializeField] private int requiredCharges;
    [SerializeField] private float chargeTime;

    private float currentChargeTime = Mathf.Infinity;
    private bool startedTeleport = true;
    private bool teleportingToMoonpool;
    private bool selected;

    private void Start()
    {
        Player.main.onTeleportationComplete += () =>
        {
            if (!teleportingToMoonpool) return;

            subRigidbody.isKinematic = false;
            Player.main.GetComponent<Collider>().enabled = true;

            Invoke(nameof(ReEnterPilotingMode), 0.25f);
        };
    }

    public void StartTeleportChargeUp()
    {
        if (!upgradeInstalled) return;

        subRoot.voiceNotificationManager.PlayVoiceNotification(emergencyWarpNotification);

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

    private void TeleportToMoonpool()
    {
        UWE.CoroutineHost.StartCoroutine(TeleportSub());
        startedTeleport = true;
    }

    private IEnumerator TeleportSub()
    {
        Player.main.AddUsedTool(TechType.PrecursorTeleporter);

        Player.main.cinematicModeActive = true;
        Player.main.playerController.inputEnabled = false;
        Inventory.main.quickSlots.SetIgnoreHotkeyInput(true);
        Player.main.GetPDA().Close();
        Player.main.GetPDA().SetIgnorePDAInput(true);
        Player.main.teleportingLoopSound.Play();
        Player.main.GetComponent<Collider>().enabled = false;

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

    private void ReEnterPilotingMode()
    {
        Player.main.EnterPilotingMode(pilotingChair);
    }

    public override bool GetUpgradeEnabled() => selected;

    public override bool OnActivated()
    {
        if (subRoot.powerRelay.GetPower() < PrototypePowerSystem.CHARGE_POWER_AMOUNT * requiredCharges)
        {
            subRoot.voiceNotificationManager.PlayVoiceNotification(insufficientPowerNotification);
            return false;
        }
        
        if (isCharging) return false;
        
        StartTeleportChargeUp();
        return true;
    }

    public override void OnSelectedChanged(bool changed)
    {
        selected = changed;
    }
}
