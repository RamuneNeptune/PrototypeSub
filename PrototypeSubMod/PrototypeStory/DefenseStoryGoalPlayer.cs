using Story;
using UnityEngine;

namespace PrototypeSubMod.PrototypeStory;

internal class DefenseStoryGoalPlayer : MonoBehaviour
{
    public void OnPlayerEnter()
    {
        if (StoryGoalManager.main.IsGoalComplete("OnMoonpoolNoPrototype")) return;

        StoryGoalManager.main.OnGoalComplete("OnApproachDefenseFacility");
    }
}
