using PrototypeSubMod.Facilities;
using Story;
using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.Materials;

internal class LightingControllerTrigger : MonoBehaviour
{
    [SerializeField] private MultipurposeAlienTerminal terminal;
    [SerializeField] private LightingController controller;
    [SerializeField] private string storyGoalCheck;
    [SerializeField] private bool disableAtStart = true;
    [SerializeField] private LightingController.LightingState targetState;
    [SerializeField] private float transitionTime;

    private void Start()
    {
        if (!string.IsNullOrEmpty(storyGoalCheck) && StoryGoalManager.main.IsGoalComplete(storyGoalCheck))
        {
            controller.SnapToState((int)targetState);
        }
        else
        {
            terminal.onTerminalInteracted += () =>
            {
                controller.LerpToState((int)targetState, transitionTime);
            };
        }
    }
}
