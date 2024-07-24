using UnityEngine;

namespace PrototypeSubMod.Monobehaviors;

internal class PlayerPilotingAnimationSetter : MonoBehaviour
{
    [SerializeField] private PilotingChair chair;
    [SerializeField] private string parameterName;

    private bool handDownRecently;

    public void UpdateAnimations()
    {
        bool val = chair.currentPlayer == Player.main || handDownRecently;

        if (Player.main.currChair == this)
        {
            val = false;
        }

        Player.main.playerAnimator.SetBool(parameterName, val);
    }

    //Called by CinematicModeTriggerBase via SendMessage
    public void HandDown()
    {
        handDownRecently = true;
        
        Invoke(nameof(ResetHandDown), 1f);
    }

    private void ResetHandDown()
    {
        handDownRecently = false;
    }
}
