using UnityEngine;

namespace PrototypeSubMod.Monobehaviors;

internal class InterfloorTeleporter : MonoBehaviour
{
    private readonly Color innerCol = new Color(0.142f, 0.047f, 0.476f, 0.333f);
    private readonly Color middleCol = new Color(0f, 0.285f, 0.904f, 0.571f);
    private readonly Color outerCol = new Color(0f, 0.285f, 0.904f, 0.238f);

    private const float FADE_IN_DURATION = 0.1f;
    private const float VFX_DURATION = 0.2f;
    private const float FADE_OUT_DURATION = 0.3f;

    [Header("Teleporting")]
    [SerializeField] private FMODAsset soundEffect;
    [SerializeField] private Transform teleportPosition;
    [SerializeField] private float teleporterCooldown = 1f;
    [SerializeField] private Collider collider;

    private bool allowedToTeleport = true;
    private float prevDuration;
    private WarpScreenFXController warpController;

    private Color originalInnerCol;
    private Color originalMiddleCol;
    private Color originalOuterCol;

    private void Start()
    {
        warpController = MainCamera.camera.GetComponent<WarpScreenFXController>();
        originalInnerCol = warpController.fx.mat.GetColor("_ColorCenter");
        originalMiddleCol = warpController.fx.mat.GetColor("_ColorStrength");
        originalOuterCol = warpController.fx.mat.GetColor("_ColorOuter");
    }

    private void OnTriggerEnter(Collider col)
    {
        if (!allowedToTeleport) return;

        if (col.gameObject != Player.main.gameObject) return;

        StartTeleportPlayer();
    }

    private void StartTeleportPlayer()
    {
        collider.enabled = false;
        Player.main.liveMixin.invincible = true;

        FMODUWE.PlayOneShot(soundEffect, teleportPosition.position, 0.25f);

        prevDuration = warpController.duration;
        warpController.duration = VFX_DURATION;

        warpController.fx.mat.SetColor("_ColorCenter", innerCol);
        warpController.fx.mat.SetColor("_ColorStrength", middleCol);
        warpController.fx.mat.SetColor("_ColorOuter", outerCol);

        warpController.StartWarp();

        Invoke(nameof(ResetDuration), FADE_IN_DURATION + VFX_DURATION + FADE_OUT_DURATION + 1f);

        allowedToTeleport = false;
        Invoke(nameof(ResetAllowedToTeleport), teleporterCooldown);

        Invoke(nameof(ActuallyTeleport), FADE_IN_DURATION + 0.1f);
    }

    private void ActuallyTeleport()
    {
        Player.main.SetPosition(teleportPosition.position, teleportPosition.rotation);
        Player.main.rigidBody.velocity = Vector3.zero;

        collider.enabled = true;
    }

    private void ResetDuration()
    {
        warpController.duration = prevDuration;

        warpController.fx.mat.SetColor("_ColorCenter", originalInnerCol);
        warpController.fx.mat.SetColor("_ColorStrength", originalMiddleCol);
        warpController.fx.mat.SetColor("_ColorOuter", originalOuterCol);
        Player.main.liveMixin.invincible = false;
    }

    private void ResetAllowedToTeleport()
    {
        allowedToTeleport = true;
    }
}
