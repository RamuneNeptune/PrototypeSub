using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.SubSystems;

internal class WatergateVehicleBlocker : MonoBehaviour
{
    [SerializeField] private Collider[] colliders;

    private void Start()
    {
        var playerColliders = Player.main.GetComponents<Collider>();

        foreach (Collider collider in colliders)
        {
            foreach (var item in playerColliders)
            {
                Physics.IgnoreCollision(collider, item, true);
            }
        }
    }
}
