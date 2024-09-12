using UnityEngine;

namespace PrototypeSubMod.DeployablesTerminal;

internal class DeployableLight : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;

    public void LaunchWithForce(float force, Vector3 previousVelocity)
    {
        rb.AddForce((transform.forward * force) + previousVelocity, ForceMode.Impulse);
    }
}
