using UnityEngine;

public class Test : MonoBehaviour
{
    public Transform target;

    private void OnDrawGizmosSelected()
    {
        if (!target) return;

        Vector3 delta = target.position - transform.position;
        float vertAngle = Mathf.Atan(delta.y / delta.z) * Mathf.Rad2Deg;
        float hozAngle = Mathf.Atan(delta.z / delta.x) * Mathf.Rad2Deg;

        Debug.Log(Vector3.Angle(transform.forward, delta.normalized));
        Gizmos.DrawLine(transform.position, target.position);

        Gizmos.color = Color.green;
        Vector3 yPos = transform.position + new Vector3(0, delta.y, 0);
        Gizmos.DrawLine(transform.position, yPos);
        Gizmos.color = Color.blue;
        Vector3 zPos = yPos + new Vector3(0, 0, delta.z);
        Gizmos.DrawLine(yPos, zPos);
        Gizmos.color = Color.red;
        Vector3 xPos = zPos + new Vector3(delta.x, 0, 0);
        Gizmos.DrawLine(zPos, xPos);
    }
}
