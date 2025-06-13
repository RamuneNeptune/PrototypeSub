using System;
using System.Collections;
using UnityEngine;

namespace PrototypeSubMod.EngineLever;

internal class ProtoEngineLever : CinematicModeTriggerBase
{
    private static readonly int PistonsActive = Animator.StringToHash("PistonsActive");
    private static readonly int LeverEnabled = Animator.StringToHash("LeverEnabled");
    private static readonly int EnabledFromSave = Animator.StringToHash("EnabledFromSave");

    public event Action<bool> onEngineStateChanged; 
    
    [SerializeField] private SubRoot subRoot;
    [SerializeField] private CyclopsMotorMode motorMode;
    [SerializeField] private EmissiveIntensityPingPong emissivePingPong;
    [SerializeField] private Animator leverAnimator;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Animator hullPistonsAnimator;
    [SerializeField] private Collider interactableCollider;
    [SerializeField] private Transform leftIKTarget;
    [SerializeField] private Transform rightIKTarget;
    [SerializeField] private FMOD_CustomEmitter startupSound;
    [SerializeField] private FMOD_CustomEmitter shutdownSound;
    [SerializeField] private VoiceNotification engineLockedNotification;
    
    private bool ensureAnimFinished;
    private bool locked;
    private bool initialized;

    private void OnEnable()
    {
        UWE.CoroutineHost.StartCoroutine(Initialize());
    }

    private IEnumerator Initialize()
    {
        if (initialized) yield break;
        
        cinematicController.animator = Player.main.playerAnimator;
        onEngineStateChanged?.Invoke(motorMode.engineOn);

        if (motorMode.engineOn)
        {
            leverAnimator.SetTrigger(EnabledFromSave);
        }

        leverAnimator.SetBool(LeverEnabled, motorMode.engineOn);
        yield return new WaitForEndOfFrame();

        emissivePingPong.SetActive(motorMode.engineOn);
        initialized = true;
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

        bool nextState = !leverAnimator.GetBool(LeverEnabled);
        leverAnimator.SetBool(LeverEnabled, nextState);
        playerAnimator.SetTrigger(nextState ? "LeverDown" : "LeverUp");
        hullPistonsAnimator.SetBool(PistonsActive, nextState);

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
        UWE.CoroutineHost.StartCoroutine(TriggerEventDelayed());
    }

    private IEnumerator TriggerEventDelayed()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        
        onEngineStateChanged?.Invoke(motorMode.engineOn);
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
