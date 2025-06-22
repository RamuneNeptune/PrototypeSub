using System.Collections;
using Story;
using UnityEngine;
using UnityEngine.Events;

namespace PrototypeSubMod.Facilities;

internal class UnlockStoryGoal : MonoBehaviour
{
    [SerializeField] private InteractableTerminal terminal;
    [SerializeField] private string storyGoalKey;
    [SerializeField] private UnityEvent onTrigger;

    private void Start()
    {
        terminal.onTerminalInteracted += () =>
        {
            StoryGoalManager.main.OnGoalComplete(storyGoalKey);
            onTrigger?.Invoke();
        };

        UWE.CoroutineHost.StartCoroutine(LateInitialize());
    }

    private IEnumerator LateInitialize()
    {
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
