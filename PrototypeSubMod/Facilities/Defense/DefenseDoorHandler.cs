using UnityEngine;

namespace PrototypeSubMod.Facilities.Defense;

internal class DefenseDoorHandler : MonoBehaviour
{
    [SerializeField] private FMODAsset doorOpenSFX;
    [SerializeField] private FMODAsset doorSwooshSFX;
    [SerializeField] private Transform openSFXPos;
    [SerializeField] private Animator animator;

    public void OpenDoor()
    {
        FMODUWE.PlayOneShot(doorOpenSFX, openSFXPos.position);
        FMODUWE.PlayOneShot(doorSwooshSFX, openSFXPos.position);
        animator.SetTrigger("OpenDoor");
    }
}
