using UnityEngine;

public class LarvaAttachPointPreview : MonoBehaviour
{
    public Mesh cubeMesh;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if (cubeMesh == null) return;

        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            Gizmos.DrawMesh(cubeMesh, 0, child.position, child.rotation, new Vector3(child.localScale.x, child.localScale.y, child.localScale.z * 0.01f));
        }
    }
}
