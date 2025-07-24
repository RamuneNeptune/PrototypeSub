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
    [SerializeField] private UnityEvent onFirstTrigger;
    [SerializeField] private int storyGoalCheckFrameDelay = 5;
    [SerializeField] private bool doubleRun;
    
    private void Start()
    {
        terminal.onTerminalInteracted += () =>
        {
            if (!StoryGoalManager.main.IsGoalComplete(storyGoalKey))
            {
                onFirstTrigger?.Invoke();
            }
            
            StoryGoalManager.main.OnGoalComplete(storyGoalKey);
            onTrigger?.Invoke();

            if (doubleRun)
            {
                UWE.CoroutineHost.StartCoroutine(LateDoubleRun());
            }
        };

        UWE.CoroutineHost.StartCoroutine(LateInitialize());
    }

    private IEnumerator LateInitialize()
    {
        for (int i = 0; i < storyGoalCheckFrameDelay; i++)
        {
            yield return new WaitForEndOfFrame();
        }
        
        if (StoryGoalManager.main.IsGoalComplete(storyGoalKey))
        {
            terminal.ForceInteracted();
        }
    }

    private IEnumerator LateDoubleRun()
    {
        yield return new WaitForSeconds(1);
        
        onTrigger?.Invoke();
    }

    public void ManualSetup(MultipurposeAlienTerminal terminal, string storyKey)
    {
        this.terminal = terminal;
        storyGoalKey = storyKey;
    }
}
