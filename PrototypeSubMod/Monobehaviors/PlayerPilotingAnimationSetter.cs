using UnityEngine;

namespace PrototypeSubMod.Monobehaviors;

internal class PlayerPilotingAnimationSetter : MonoBehaviour
{
    [SerializeField] private PilotingChair chair;
    [SerializeField] private string parameterName;

    private bool handDownRecently;
    private bool startedPiloting;

    public void OnAnimationStarted()
    {
        bool value = handDownRecently;

        Plugin.Logger.LogInfo($"Setting param to {value}");
        Player.main.playerAnimator.SetBool(parameterName, value);

        startedPiloting = true;
    }

    public void OnAnimationEnded()
    {
        if (startedPiloting)
        {
            startedPiloting = false;
            Plugin.Logger.LogInfo($"Started piloting = true. Returning");
            return;
        }

        Plugin.Logger.LogInfo($"Setting param false");
        Player.main.playerAnimator.SetBool(parameterName, false);
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
