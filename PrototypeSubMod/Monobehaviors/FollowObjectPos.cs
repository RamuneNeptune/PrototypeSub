using UnityEngine;

namespace PrototypeSubMod.Monobehaviors;

internal class FollowObjectPos : MonoBehaviour
{
    [SerializeField] private Transform followObj;

    private void LateUpdate()
    {
        transform.position = followObj.position;
    }
}
