using System.Collections;
using Story;
using UnityEngine;

namespace PrototypeSubMod.PrototypeStory;

internal class DefenseStoryGoalPlayer : MonoBehaviour
{
    public void OnPlayerEnter()
    {
        UWE.CoroutineHost.StartCoroutine(WaitAndPlayVoiceline());
    }

    private IEnumerator WaitAndPlayVoiceline()
    {
        yield return new WaitUntil(() => LargeWorldStreamer.main.IsWorldSettled());

        yield return new WaitForEndOfFrame();
        
        if (StoryGoalManager.main.IsGoalComplete("OnMoonpoolNoPrototype")) yield break;

        if (AtmosphereDirector.main.GetBiomeOverride() == "defensefacilityteleporterroom") yield break;
        
        StoryGoalManager.main.OnGoalComplete("OnApproachDefenseFacility");
    }
}
