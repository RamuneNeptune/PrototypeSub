using UnityEngine;

namespace PrototypeSubMod.EngineLever;

internal class ProtoEngineLever : CinematicModeTriggerBase
{
    [SerializeField] private Animator leverAnimator;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Collider interactableCollider;
    [SerializeField] private CyclopsMotorMode motorMode;
    [SerializeField] private Transform leftIKTarget;
    [SerializeField] private Transform rightIKTarget;
    [SerializeField] private FMOD_CustomEmitter startupSound;
    [SerializeField] private FMOD_CustomEmitter shutdownSound;

    private bool ensureAnimFinished;

    private void Start()
    {
        cinematicController.animator = Player.main.playerAnimator;
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
        playerAnimator.SetTrigger(nextState ? "LeverDown" : "LeverUp");

        if (nextState)
        {
            startupSound.Play();
        }
        else
        {
            shutdownSound.Play();
        }
    }
    
    public void OnCinematicEnd()
    {
        Player.main.armsController.SetWorldIKTarget(null, null);
        interactableCollider.enabled = false;
        ensureAnimFinished = true;

        UWE.CoroutineHost.StartCoroutine(motorMode.ChangeEngineState(true));
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
