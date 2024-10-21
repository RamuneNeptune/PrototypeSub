using UnityEngine;

namespace PrototypeSubMod.Monobehaviors;

internal class FollowObjectPos : MonoBehaviour
{
    [SerializeField] private Transform followObj;
    [SerializeField] private Vector3 localOffset;

    private void LateUpdate()
    {
        transform.localPosition = transform.parent.InverseTransformPoint(followObj.position) + localOffset;
    }
}
