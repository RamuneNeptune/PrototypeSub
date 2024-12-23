using UnityEngine;
using UnityEditor;

public class BotPointPreview : MonoBehaviour
{
    [SerializeField] private bool showPreview;

    private void OnDrawGizmos()
    {
        if (!showPreview) return;

        Gizmos.color = Color.cyan;

        for (int i = 0; i < transform.childCount; i++)
        {
            var childA = transform.GetChild(i);
            Gizmos.DrawCube(childA.position, Vector3.one * 0.25f);
        }
    }
}
