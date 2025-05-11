using System;
using Story;
using UnityEngine;

namespace PrototypeSubMod.Facilities;

public class IonCubeTerminalManager : MonoBehaviour
{
    [SerializeField] private MultipurposeIonCubeTerminal terminal;
    [SerializeField] private string storyGoal;

    private void Start()
    {
        if (StoryGoalManager.main.IsGoalComplete(storyGoal))
        {
            terminal.ForceInteracted();
        }
    }

    public void TriggerStoryGoal()
    {
        StoryGoalManager.main.OnGoalComplete(storyGoal);
    }
}