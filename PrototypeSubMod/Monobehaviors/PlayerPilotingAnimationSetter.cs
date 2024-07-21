using UnityEngine;

namespace PrototypeSubMod.Monobehaviors;

internal class PlayerPilotingAnimationSetter : MonoBehaviour
{
    [SerializeField] private PilotingChair chair;
    [SerializeField] private string parameterName;

    private bool handDownRecently;

    public void UpdateAnimations()
    {
        Player.main.playerAnimator.SetBool(parameterName, chair.currentPlayer == Player.main || handDownRecently);
    }

    //Called by CinematicModeTriggerBase via SendMessage
    public void HandDown()
    {
        handDownRecently = true;

        Invoke(nameof(ResetHandDown), .75f);
    }

    private void ResetHandDown()
    {
        handDownRecently = false;
    }
}
