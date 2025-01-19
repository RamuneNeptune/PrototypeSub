using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors;

internal class SetRBCenterOfMass : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Vector3 localOffset;

    private void Start()
    {
        rb.centerOfMass = localOffset;
    }
}
