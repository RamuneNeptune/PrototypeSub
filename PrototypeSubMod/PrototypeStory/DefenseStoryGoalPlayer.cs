using Story;
using UnityEngine;

namespace PrototypeSubMod.PrototypeStory;

internal class DefenseStoryGoalPlayer : MonoBehaviour
{
    public void OnPlayerEnter()
    {
        if (Plugin.GlobalSaveData.visitedDefenseMoonpool) return;

        StoryGoalManager.main.OnGoalComplete("OnApproachDefenseFacility");
    }
}
