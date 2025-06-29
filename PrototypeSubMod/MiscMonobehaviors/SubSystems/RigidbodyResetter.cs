using System.Collections;
using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.SubSystems;

public class RigidbodyResetter : MonoBehaviour
{
    [SerializeField] private Rigidbody rigidbody;

    private Vector3 centerOfMass;
    private Vector3 inertiaTensor;
    private Quaternion intertiaRotation;

    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();

        centerOfMass = rigidbody.centerOfMass;
        inertiaTensor = rigidbody.inertiaTensor;
        intertiaRotation = rigidbody.inertiaTensorRotation;
    }

    private void FixedUpdate()
    {
        if (inertiaTensor == Vector3.zero) return;

        rigidbody.centerOfMass = centerOfMass;
        rigidbody.inertiaTensor = inertiaTensor;
        rigidbody.inertiaTensorRotation = intertiaRotation;
    }
}