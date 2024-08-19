using System.Collections;
using UnityEngine;

namespace PrototypeSubMod.Monobehaviors;

internal class InterfloorTeleporter : MonoBehaviour
{
    [SerializeField] private FMODAsset soundEffect;
    [SerializeField] private Transform teleportPosition;
    [SerializeField] private float teleporterCooldown = 1f;

    private bool allowedToTeleport = true;

    private void OnTriggerEnter(Collider col)
    {
        if (!allowedToTeleport) return;

        if (col.gameObject != Player.main.gameObject) return;

        Player.main.SetPosition(teleportPosition.position, teleportPosition.rotation);
        Player.main.mode = Player.Mode.Sitting;
        Player.main.rigidBody.velocity = Vector3.zero;

        FMODUWE.PlayOneShot(soundEffect, teleportPosition.position, 0.5f);

        MainCamera.camera.GetComponent<WarpScreenFXController>().StartWarp();

        StopCoroutine(DelayedCancelWarp());
        StartCoroutine(DelayedCancelWarp());

        allowedToTeleport = false;
        Invoke(nameof(ResetAllowedToTeleport), teleporterCooldown);
    }

    private void ResetAllowedToTeleport()
    {
        allowedToTeleport = true;
    }

    private IEnumerator DelayedCancelWarp()
    {
        yield return new WaitForSecondsRealtime(0.2f);

        Player.main.mode = Player.Mode.Normal;
        MainCamera.camera.GetComponent<WarpScreenFXController>().StopWarp();
    }
}
