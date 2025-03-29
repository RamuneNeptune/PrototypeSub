using System.Collections;
using Story;
using UnityEngine;

namespace PrototypeSubMod.Facilities;

internal class UnlockStoryGoal : MonoBehaviour
{
    [SerializeField] private MultipurposeAlienTerminal terminal;
    [SerializeField] private string storyGoalKey;

    private IEnumerator Start()
    {
        terminal.onTerminalInteracted += () =>
        {
            StoryGoalManager.main.OnGoalComplete(storyGoalKey);
        };

        for (int i = 0; i < 5; i++)
        {
            yield return new WaitForEndOfFrame();
        }
        
        if (StoryGoalManager.main.IsGoalComplete(storyGoalKey))
        {
            terminal.ForceInteracted();
        }
    }
}
