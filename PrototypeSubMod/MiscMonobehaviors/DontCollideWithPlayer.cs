using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors;

public class DontCollideWithPlayer : MonoBehaviour
{
    private void Start()
    {
        foreach (var col in GetComponentsInChildren<Collider>(true))
        {
            Physics.IgnoreCollision(Player.mainCollider, col);
        }
    }
}