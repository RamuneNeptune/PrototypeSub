using Story;
using UnityEngine;

namespace PrototypeSubMod.Facilities.Defense;

internal class MoonpoolDoorManager : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private FMOD_CustomEmitter openSFX;
    [SerializeField] private PlayerDistanceTracker playerDistanceTracker;
    [SerializeField] private float checkIntermittance = 5f;
    [SerializeField] private float noEntryPlayerDistance = 50f;

    private void Start()
    {
        if (Plugin.GlobalSaveData.moonpoolDoorOpened)
        {
            animator.SetTrigger("OpenDoor");
        }
        else
        {
            InvokeRepeating(nameof(CheckIfPlayerClose), 0, checkIntermittance);
        }
    }

    public void OpenDoor()
    {
        if (Plugin.GlobalSaveData.moonpoolDoorOpened) return;

        animator.SetTrigger("OpenDoor");
        openSFX.Play();
        Plugin.GlobalSaveData.moonpoolDoorOpened = true;
        CancelInvoke();
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
}
