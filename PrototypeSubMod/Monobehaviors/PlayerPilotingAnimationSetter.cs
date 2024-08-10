using UnityEngine;

namespace PrototypeSubMod.Monobehaviors;

internal class PlayerPilotingAnimationSetter : MonoBehaviour
{
    [SerializeField] private PilotingChair chair;
    [SerializeField] private string parameterName;

    private Player playerLastFrame;
    private bool handDownRecently;

    public void OnAnimationStart(CinematicModeEventData eventData)
    {
        Player.main.playerAnimator.SetBool(parameterName, handDownRecently);
    }

    private void Update()
    {
        if(chair.currentPlayer != playerLastFrame)
        {
            Player.main.playerAnimator.SetBool(parameterName, chair.currentPlayer != null);
        }

        playerLastFrame = chair.currentPlayer;
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
