using System;
using System.Collections;
using UnityEngine;

namespace PrototypeSubMod.Teleporter;

internal class ProtoEmergencyWarp : MonoBehaviour
{
    public static bool isCharging;

    private Vector3 SUB_TELEPORT_POSITION { get; } = new Vector3(464.339f, -106.670f, 1213.019f);
    private const float TELEPORT_ANGLE = 20f;

    [SerializeField] private Rigidbody subRigidbody;
    [SerializeField] private SubRoot subRoot;
    [SerializeField] private PilotingChair pilotingChair;
    [SerializeField] private int requiredPower;
    [SerializeField] private float chargeTime;

    private float currentChargeTime = Mathf.Infinity;
    private bool startedTeleport = true;
    private bool teleportingToMoonpool;

    private void Start()
    {
        Player.main.onTeleportationComplete += () =>
        {
            if (!teleportingToMoonpool) return;

            subRigidbody.isKinematic = false;

            Invoke(nameof(ReEnterPilotingMode), 0.5f);
        };
    }

    public void TryStartTeleportChargeUp()
    {
        if (subRoot.powerRelay.GetPower() < requiredPower)
        {
            throw new NotImplementedException("No power voiceline not yet implemented");
        }

        subRoot.powerRelay.ConsumeEnergy(requiredPower, out _);
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

        Camera.main.GetComponent<TeleportScreenFXController>().StartTeleport();
        subRigidbody.isKinematic = true;
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
}
