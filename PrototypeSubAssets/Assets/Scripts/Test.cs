using UnityEngine;

public class Test : MonoBehaviour
{
    public Transform center;

    public void OnDrawGizmos()
    {
        if (center == null) return;

        Vector3 pos = Vector3.ProjectOnPlane(transform.forward + transform.position, center.forward);
        pos += center.position;
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(pos, 0.2f);

        Vector3 dir = pos - center.position;
        dir.Normalize();
        Gizmos.DrawRay(center.position, dir);

        Vector3 localDir = center.InverseTransformDirection(dir);
        //Debug.Log(localDir);
    }
}
