using UnityEngine;

namespace PrototypeSubMod.Facilities.Defense;

internal class ProtoDoorTransmitter : MonoBehaviour
{
    private static readonly Vector3 MoonpoolPos = new Vector3(782.1522f, -334.874451f, -1046.78748f);

    [SerializeField] private VoiceNotification openDoorsNotification;
    [SerializeField] private SubRoot subRoot;
    [SerializeField] private float activationDistance;
    [SerializeField] private float playerCheckInterval = 5f;

    private MoonpoolDoorManager doorManager;

    private void Start()
    {
        InvokeRepeating(nameof(CheckPlayerPos), 0, playerCheckInterval);
    }

    private void CheckPlayerPos()
    {
        if (Plugin.GlobalSaveData.moonpoolDoorOpened)
        {
            CancelInvoke(nameof(CheckPlayerPos));
            return;
        }

        float distToDoor = Vector3.Distance(Camera.main.transform.position, MoonpoolPos);
        if (distToDoor < activationDistance)
        {
            OpenDoor();
        }
    }

    private void GetDoorReference()
    {
        if (doorManager != null) return;

        var colliders = Physics.OverlapSphere(MoonpoolPos, 5);
        foreach (var collider in colliders)
        {
            var manager = collider.GetComponentInParent<MoonpoolDoorManager>();
            if (!manager) continue;

            doorManager = manager;
            break;
        }

        if (doorManager == null)
        {
            throw new System.Exception("Moonpool door manager not found. Large world not settled?");
        }
    }

    private void OpenDoor()
    {
        GetDoorReference();
        subRoot.voiceNotificationManager.PlayVoiceNotification(openDoorsNotification);
        doorManager.OpenDoor();
    }
}
