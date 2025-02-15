using UnityEngine;

public class Test : MonoBehaviour
{
    public MeshFilter filter;
    public Transform testObj;
    public float range;
    public float offset;
    public float scale;
    public bool enabled;

    private void OnDrawGizmosSelected()
    {
        if (!enabled) return;

        if (!filter || !testObj) return;

        Mesh mesh = filter.sharedMesh;
        if (mesh == null) return;

        ShowTangentSpace(mesh);
    }

    private void ShowTangentSpace(Mesh mesh)
    {
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;
        Vector4[] tangents = mesh.tangents;
        for (int i = 0; i < vertices.Length; i++)
        {
            if (vertices[i].magnitude > range * 0.01f) continue;

            ShowTangentSpace(
                transform.TransformPoint(vertices[i]),
                transform.TransformDirection(normals[i]),
                transform.TransformDirection(tangents[i]),
                tangents[i].w
            );
        }
    }

    private void ShowTangentSpace(Vector3 vertex, Vector3 normal, Vector3 tangent, float binormalSign)
    {
        vertex += normal * offset;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(vertex, vertex + normal * scale);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(vertex, vertex + tangent * scale);

        Vector3 binormal = Vector3.Cross(normal, tangent) * binormalSign;
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(vertex, vertex + binormal * scale);
    }
}
