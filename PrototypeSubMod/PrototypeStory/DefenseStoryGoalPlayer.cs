using System;
using System.Collections;
using PrototypeSubMod.Facilities.Defense;
using Story;
using UnityEngine;

namespace PrototypeSubMod.PrototypeStory;

internal class DefenseStoryGoalPlayer : MonoBehaviour
{
    private DefenseTeleporterRoomManager teleporterRoomManager;
    
    private void Start()
    {
        teleporterRoomManager = FindObjectOfType<DefenseTeleporterRoomManager>();
    }

    public void OnPlayerEnter()
    {
        UWE.CoroutineHost.StartCoroutine(WaitAndPlayVoiceline());
    }

    private IEnumerator WaitAndPlayVoiceline()
    {
        yield return new WaitUntil(() => LargeWorldStreamer.main.IsWorldSettled());

        yield return new WaitForEndOfFrame();
        
        if (StoryGoalManager.main.IsGoalComplete("OnMoonpoolNoPrototype")) yield break;

        if (teleporterRoomManager.PlayerInRoom()) yield break;
        
        StoryGoalManager.main.OnGoalComplete("OnApproachDefenseFacility");
    }
}
