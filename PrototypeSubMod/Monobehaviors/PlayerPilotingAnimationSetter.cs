using UnityEngine;

namespace PrototypeSubMod.Monobehaviors;

internal class PlayerPilotingAnimationSetter : MonoBehaviour
{
    [SerializeField] private PilotingChair chair;
    [SerializeField] private string parameterName;

    private bool handDownRecently;
    private bool wasPiloting;
    int timesWasntPiloting = 0;

    public void UpdateAnimations()
    {
        bool val = handDownRecently;
        if (wasPiloting && timesWasntPiloting >= 1)
        {
            val = false;
            timesWasntPiloting = 0;
        }

        if (!wasPiloting) timesWasntPiloting++;

        Plugin.Logger.LogInfo($"Updating animations. Hand down recently = {handDownRecently} | Was piloting = {wasPiloting}");

        Player.main.playerAnimator.SetBool(parameterName, val);

        wasPiloting = val;
    }

    //Called by CinematicModeTriggerBase via SendMessage
    public void HandDown()
    {
        handDownRecently = true;

        Invoke(nameof(ResetHandDown), .8f);
    }

    private void ResetHandDown()
    {
        handDownRecently = false;
    }
}
