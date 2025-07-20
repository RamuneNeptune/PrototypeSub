using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.SubSystems;

public class DockingBayBounds : MonoBehaviour
{
    [SerializeField] private Transform gizmoTarget;
    [SerializeField] private Vector3 bounds;
    
    public Vector3 GetBounds() => bounds;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(gizmoTarget.position, bounds);
    }
}