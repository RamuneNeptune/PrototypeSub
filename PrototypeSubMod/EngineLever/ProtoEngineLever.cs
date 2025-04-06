using System.Collections;
using UnityEngine;

namespace PrototypeSubMod.EngineLever;

internal class ProtoEngineLever : CinematicModeTriggerBase
{
    private static readonly int EngineOn = Animator.StringToHash("FinsActive");
    
    [SerializeField] private SubRoot subRoot;
    [SerializeField] private CyclopsMotorMode motorMode;
    [SerializeField] private EmissiveIntensityPingPong emissivePingPong;
    [SerializeField] private Animator leverAnimator;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Transform leftFinsParent;
    [SerializeField] private Transform rightFinsParent;
    [SerializeField] private Collider interactableCollider;
    [SerializeField] private Transform leftIKTarget;
    [SerializeField] private Transform rightIKTarget;
    [SerializeField] private FMOD_CustomEmitter startupSound;
    [SerializeField] private FMOD_CustomEmitter shutdownSound;
    [SerializeField] private VoiceNotification engineLockedNotification;

    private Animator[] leftFinAnimators;
    private Animator[] rightFinAnimators;
    private bool ensureAnimFinished;
    private bool locked;

    private IEnumerator Start()
    {
        cinematicController.animator = Player.main.playerAnimator;
        leftFinAnimators = leftFinsParent.GetComponentsInChildren<Animator>();
        rightFinAnimators = rightFinsParent.GetComponentsInChildren<Animator>();
        StartCoroutine(UpdateFinState(motorMode.engineOn));

        if (motorMode.engineOn)
        {
            leverAnimator.SetTrigger("EnabledFromSave");
        }

        leverAnimator.SetBool("LeverEnabled", motorMode.engineOn);
        yield return new WaitForEndOfFrame();

        emissivePingPong.SetActive(motorMode.engineOn);
    }

    public override void OnHandHover(GUIHand hand)
    {
        string key = motorMode.engineOn ? "ProtoEngineLeverDisable" : "ProtoEngineLeverEnable";
        var main = HandReticle.main;
        main.SetText(HandReticle.TextType.Hand, key, true, GameInput.Button.LeftHand);
        main.SetText(HandReticle.TextType.HandSubscript, string.Empty, false, GameInput.Button.None);
        main.SetIcon(HandReticle.IconType.Hand, 1f);
    }

    public override void OnHandClick(GUIHand hand)
    {
        if (locked)
        {
            subRoot.voiceNotificationManager.PlayVoiceNotification(engineLockedNotification);
            return;
        }

        base.OnHandClick(hand);
    }

    public override void OnStartCinematicMode()
    {
        base.OnStartCinematicMode();
        Player.main.armsController.SetWorldIKTarget(leftIKTarget, rightIKTarget);

        bool nextState = !leverAnimator.GetBool("LeverEnabled");
        leverAnimator.SetBool("LeverEnabled", nextState);
        StartCoroutine(UpdateFinState(nextState));
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

    private IEnumerator UpdateFinState(bool targetState)
    {
        for (int i = 0; i < leftFinAnimators.Length; i++)
        {
            var animL = leftFinAnimators[i];
            var animR = rightFinAnimators[i];
            animL.SetBool(EngineOn, targetState);
            animR.SetBool(EngineOn, targetState);

            yield return new WaitForSeconds(0.5f);
        }
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

    public void SetStoryLocked(bool locked)
    {
        this.locked = locked;
    }
}
