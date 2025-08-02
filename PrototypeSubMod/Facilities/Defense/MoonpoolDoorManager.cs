using Story;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PrototypeSubMod.Facilities.Defense;

internal class MoonpoolDoorManager : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private FMOD_CustomEmitter openSFX;
    [SerializeField] private FMOD_CustomEmitter locationSignalSearching;
    [SerializeField] private FMOD_CustomEmitter locationSignalFound;
    [SerializeField] private PlayerDistanceTracker playerDistanceTracker;
    [SerializeField] private float checkIntermittance = 5f;
    [SerializeField] private float noEntryPlayerDistance = 50f;
    [SerializeField] private float searchSoundIntermittance = 5f;

    private bool playerFound;

    private void OnEnable()
    {
        if (Plugin.GlobalSaveData.moonpoolDoorOpened)
        {
            animator.SetTrigger("InstantOpen");
        }
        else
        {
            CancelInvoke(nameof(CheckIfPlayerClose));
            CancelInvoke(nameof(PlaySearchSound));
            InvokeRepeating(nameof(CheckIfPlayerClose), 0, checkIntermittance);
            InvokeRepeating(nameof(PlaySearchSound), 0, searchSoundIntermittance);
        }
    }

    public void OpenDoor()
    {
        if (Plugin.GlobalSaveData.moonpoolDoorOpened) return;

        animator.SetTrigger("OpenDoor");
        openSFX.Play();

        playerFound = true;

        Plugin.GlobalSaveData.moonpoolDoorOpened = true;
        CancelInvoke(nameof(CheckIfPlayerClose));
    }

    private void CheckIfPlayerClose()
    {
        if (!Plugin.GlobalSaveData.EngineFacilityPointsRepaired) return;

        if (playerDistanceTracker.distanceToPlayer > noEntryPlayerDistance) return;

        if (Player.main.currentSub != null)
        {
            var doorTransmitter = Player.main.currentSub.GetComponentInChildren<ProtoDoorTransmitter>();
            if (doorTransmitter != null) return;
        }

        StoryGoalManager.main.OnGoalComplete("OnMoonpoolNoPrototype");
    }

    private void PlaySearchSound()
    {
        if (!playerFound)
        {
            locationSignalSearching.Play();
        }
        else
        {
            locationSignalFound.Play();
            CancelInvoke(nameof(PlaySearchSound));
        }
    }
}
