using UnityEngine;

namespace PrototypeSubMod.DeployablesTerminal;

internal class DeployableLight : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;

    public void LaunchWithForce(float force)
    {
        rb.AddForce(transform.forward * force, ForceMode.Impulse);
    }
}
