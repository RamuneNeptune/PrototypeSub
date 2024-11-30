using UnityEngine;

namespace PrototypeSubMod.EngineLever;

internal class ProtoEngineLever : CinematicModeTriggerBase
{
    [SerializeField] private SubRoot subRoot;
    [SerializeField] private CyclopsMotorMode motorMode;
    [SerializeField] private EmissiveIntensityPingPong emissivePingPong;
    [SerializeField] private Animator leverAnimator;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Animator finsAnimator;
    [SerializeField] private Collider interactableCollider;
    [SerializeField] private Transform leftIKTarget;
    [SerializeField] private Transform rightIKTarget;
    [SerializeField] private FMOD_CustomEmitter startupSound;
    [SerializeField] private FMOD_CustomEmitter shutdownSound;

    private bool ensureAnimFinished;

    private void Start()
    {
        cinematicController.animator = Player.main.playerAnimator;
        finsAnimator.SetBool("EngineOn", motorMode.engineOn);
        emissivePingPong.SetActive(motorMode.engineOn);

        if (motorMode.engineOn)
        {
            leverAnimator.SetTrigger("EnabledFromSave");
        }
    }

    public override void OnHandHover(GUIHand hand)
    {
        string key = motorMode.engineOn ? "ProtoEngineLeverDisable" : "ProtoEngineLeverEnable";
        var main = HandReticle.main;
        main.SetText(HandReticle.TextType.Hand, key, true, GameInput.Button.LeftHand);
        main.SetText(HandReticle.TextType.HandSubscript, string.Empty, false, GameInput.Button.None);
        main.SetIcon(HandReticle.IconType.Hand, 1f);
    }

    public override void OnStartCinematicMode()
    {
        base.OnStartCinematicMode();
        Player.main.armsController.SetWorldIKTarget(leftIKTarget, rightIKTarget);

        bool nextState = !leverAnimator.GetBool("LeverEnabled");
        leverAnimator.SetBool("LeverEnabled", nextState);
        finsAnimator.SetBool("EngineOn", nextState);
        playerAnimator.SetTrigger(nextState ? "LeverDown" : "LeverUp");

        if (nextState)
        {
            startupSound.Play();
            subRoot.voiceNotificationManager.PlayVoiceNotification(subRoot.enginePowerUpNotification);
        }
        else
        {
            shutdownSound.Play();
            subRoot.voiceNotificationManager.PlayVoiceNotification(subRoot.enginePowerDownNotification);
        }
    }

    public void OnCinematicEnd()
    {
        Player.main.armsController.SetWorldIKTarget(null, null);
        interactableCollider.enabled = false;
        ensureAnimFinished = true;

        UWE.CoroutineHost.StartCoroutine(motorMode.ChangeEngineState(!motorMode.engineOn));
    }

    private void Update()
    {
        if (ensureAnimFinished)
        {
            bool currentlyEnabled = leverAnimator.GetCurrentAnimatorStateInfo(0).IsName("LeverEnabled");
            bool currentlyDisabled = leverAnimator.GetCurrentAnimatorStateInfo(0).IsName("LeverDisabled");
            if (currentlyEnabled || currentlyDisabled)
            {
                ensureAnimFinished = false;
                interactableCollider.enabled = true;
            }
        }
    }
}
