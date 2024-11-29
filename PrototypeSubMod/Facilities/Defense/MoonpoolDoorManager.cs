using UnityEngine;

namespace PrototypeSubMod.Facilities.Defense;

internal class MoonpoolDoorManager : MonoBehaviour
{
    [SerializeField] private Animator animator;

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
        Plugin.GlobalSaveData.moonpoolDoorOpened = true;
    }
}
