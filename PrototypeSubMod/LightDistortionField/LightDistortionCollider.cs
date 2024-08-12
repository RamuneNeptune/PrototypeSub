using UnityEngine;

namespace PrototypeSubMod.LightDistortionField;

[RequireComponent(typeof(Collider))]
internal class LightDistortionCollider : MonoBehaviour
{
    public static bool PlayerInField { get; private set; }

    private void OnTriggerEnter(Collider col)
    {
        if (UWE.Utils.GetRootRigidbody(col.gameObject) == Player.main.rigidBody)
        {
            PlayerInField = true;
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (UWE.Utils.GetRootRigidbody(col.gameObject) == Player.main.rigidBody)
        {
            PlayerInField = false;
        }
    }
}
