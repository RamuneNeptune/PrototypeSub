using UnityEngine;

namespace PrototypeSubMod.EngineLever;

internal class ProtoEngineLever : CinematicModeTriggerBase
{
    [SerializeField] private Animator leverAnimator;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private CyclopsMotorMode motorMode;
    [SerializeField] private Transform leftIKTarget;
    [SerializeField] private Transform rightIKTarget;

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
    }
    
    public void OnCinematicEnd()
    {
        Player.main.armsController.SetWorldIKTarget(null, null);
    }
}
