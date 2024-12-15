using System.Collections;
using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors;

internal class SetPickupsToFall : MonoBehaviour
{
    [SerializeField] private Transform root;
    [SerializeField] private Vector3 localOffset;
    [SerializeField] private Vector3 halfExtents;

    private IEnumerator Start()
    {
        yield return new WaitUntil(LargeWorldStreamer.main.IsWorldSettled);

        int numItems = UWE.Utils.OverlapBoxIntoSharedBuffer(transform.position + localOffset, halfExtents, root.rotation);
        for (int i = 0; i < numItems; i++)
        {
            Collider col = UWE.Utils.sharedColliderBuffer[i];
            var rb = col.GetComponentInParent<Rigidbody>();

            if (rb == null) continue;

            if (rb.GetComponentInParent<SubRoot>() != null) continue;

            UWE.Utils.SetIsKinematicAndUpdateInterpolation(rb, false);

            var pickupable = col.GetComponentInParent<Pickupable>();
            if (pickupable != null)
            {
                pickupable.isKinematic = PickupableKinematicState.NonKinematic;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(localOffset, halfExtents * 2);
    }
}
