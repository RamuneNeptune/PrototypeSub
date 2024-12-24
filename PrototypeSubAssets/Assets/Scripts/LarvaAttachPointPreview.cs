using UnityEngine;

public class LarvaAttachPointPreview : MonoBehaviour
{
    public Mesh cubeMesh;
    public bool flipOrientations;

    private void OnDrawGizmos()
    {
        if (cubeMesh == null) return;

        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);

            if (flipOrientations)
            {
                child.Rotate(new Vector3(0, 180, 0), Space.Self);
            }

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(child.position, 1);

            Gizmos.color = Color.green;
            Gizmos.DrawMesh(cubeMesh, 0, child.position, child.rotation, new Vector3(child.localScale.x, child.localScale.y, child.localScale.z * 0.01f));

            Gizmos.color = Color.blue;
            Gizmos.DrawRay(child.position, -child.forward);
        }

        if (flipOrientations)
        {
            flipOrientations = false;
        }
    }
}
