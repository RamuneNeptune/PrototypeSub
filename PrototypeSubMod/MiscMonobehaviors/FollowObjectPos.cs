using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors;

internal class FollowObjectPos : MonoBehaviour
{
    [SerializeField] private Transform followObj;

    private void LateUpdate()
    {
        transform.position = followObj.position;
    }
}
