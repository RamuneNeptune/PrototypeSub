using UnityEngine;

namespace PrototypeSubMod.Facilities.Defense;

internal class DefenseDoorHandler : MonoBehaviour
{
    [SerializeField] private FMODAsset doorOpenSFX;
    [SerializeField] private FMODAsset doorSwooshSFX;
    [SerializeField] private Transform openSFXPos;
    [SerializeField] private Animator animator;

    private bool hasOpened;

    public void OpenDoor()
    {
        if (hasOpened) return;

        FMODUWE.PlayOneShot(doorOpenSFX, openSFXPos.position);
        animator.SetTrigger("OpenDoor");

        Invoke(nameof(PlaySwoosh), 4.5f);
        hasOpened = true;
    }

    private void PlaySwoosh()
    {
        FMODUWE.PlayOneShot(doorSwooshSFX, openSFXPos.position);
    }
}
