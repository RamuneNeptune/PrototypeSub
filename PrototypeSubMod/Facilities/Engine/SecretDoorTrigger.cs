using PrototypeSubMod.Prefabs;
using UnityEngine;

namespace PrototypeSubMod.Facilities.Engine;

public class SecretDoorTrigger : MonoBehaviour
{
    private static readonly int Door = Animator.StringToHash("OpenDoor");
    
    [SerializeField] private Animator animator;
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.Equals(Player.mainCollider)) return;

        var chip1 = Inventory.main.equipment.GetItemInSlot("Chip1");
        var chip2 = Inventory.main.equipment.GetItemInSlot("Chip2");
        if (chip1 == null && chip2 == null) return;

        if (chip1 != null && chip1.techType == ListeningDevice_Craftable.prefabInfo.TechType)
        {
            OpenDoor();
            return;
        }

        if (chip2 != null && chip2.techType == ListeningDevice_Craftable.prefabInfo.TechType)
        {
            OpenDoor();
        }
    }

    private void OpenDoor()
    {
        animator.SetTrigger(Door);
    }
}