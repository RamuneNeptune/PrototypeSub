using Story;
using UnityEngine;

namespace PrototypeSubMod.Facilities;

internal class UnlockStoryGoal : MonoBehaviour
{
    [SerializeField] private MultipurposeAlienTerminal terminal;
    [SerializeField] private string storyGoalKey;

    private void Start()
    {
        if (StoryGoalManager.main.IsGoalComplete(storyGoalKey))
        {
            terminal.ForceInteracted();
        }
        
        terminal.onTerminalInteracted += () =>
        {
            StoryGoalManager.main.OnGoalComplete(storyGoalKey);
        };
    }
}
