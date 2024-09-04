using UnityEngine;

public class Test : MonoBehaviour
{
    public Transform center;
    [Range(-90, 90)] public float surfaceAngleTolerance;

    public void OnDrawGizmos()
    {
        if (center == null) return;

        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, center.position);
        Gizmos.DrawRay(transform.position, transform.forward);

        Vector3 dirToCenter = (center.position - transform.position).normalized;
        float dot = Vector3.Dot(dirToCenter, transform.forward);
        Gizmos.color = dot < -(surfaceAngleTolerance / 90f) ? Color.green : Color.red;
        Debug.Log(dot);

        Gizmos.DrawSphere(transform.position, 0.1f);
    }
}
