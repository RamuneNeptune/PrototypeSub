using UnityEngine;

namespace PrototypeSubMod.Facilities.Defense;

public class DefenseTeleporterRoomManager : MonoBehaviour
{
    [SerializeField] private Collider[] roomBounds;

    public bool PlayerInRoom()
    {
        foreach (var bounds in roomBounds)
        {
            if (bounds.bounds.Contains(Player.main.transform.position))
            {
                return true;
            }
        }

        return false;
    }
}