using UnityEngine;

namespace PrototypeSubMod.Facilities.Defense;

internal class MoonpoolDoorManager : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private FMOD_CustomEmitter openSFX;

    private void Start()
    {
        if (Plugin.GlobalSaveData.moonpoolDoorOpened)
        {
            animator.SetTrigger("OpenDoor");
        }
    }

    public void OpenDoor()
    {
        if (Plugin.GlobalSaveData.moonpoolDoorOpened) return;

        animator.SetTrigger("OpenDoor");
        openSFX.Play();
        Plugin.GlobalSaveData.moonpoolDoorOpened = true;
    }
}
