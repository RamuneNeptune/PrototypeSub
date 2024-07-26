using Newtonsoft.Json.Linq;
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

        Plugin.Logger.LogInfo($"Hand down recently = {handDownRecently} | Started piloting = {startedPiloting}");

        Player.main.playerAnimator.SetBool(parameterName, value);

        startedPiloting = true;
    }

    public void OnAnimationEnded()
    {
        Plugin.Logger.LogInfo($"Started piloting = {startedPiloting}");

        if (startedPiloting)
        {
            startedPiloting = false;
            return;
        }

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
