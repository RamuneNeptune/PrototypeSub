using UnityEngine;
using UnityEditor;

public class BotPathPreview : MonoBehaviour
{
    [SerializeField] private bool showPreview;

    private void OnDrawGizmos()
    {
        if (!showPreview) return;

        if (transform.childCount <= 1) return;

        for (int i = 0; i < transform.childCount; i++)
        {
            Gizmos.color = Color.blue;

            var childA = transform.GetChild(i);
            var childB = transform.GetChild((i + 1) % transform.childCount);
            Gizmos.DrawLine(childA.position, childB.position);

            Gizmos.color = Color.cyan;
            Gizmos.DrawCube(childA.position, Vector3.one * 0.25f);
        }
    }
}
