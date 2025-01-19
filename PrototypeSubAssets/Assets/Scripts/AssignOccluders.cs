using UnityEngine;

[ExecuteInEditMode]
public class AssignOccluders : MonoBehaviour
{
    [SerializeField] private bool remove;

    private void Update()
    {
        if (!remove) return;
        remove = false;
        var occluders = GetComponentsInChildren<CullingOccluder>(true);
        foreach (var item in occluders)
        {
            item.occluderRenderer = item.GetComponent<Renderer>();
            item.meshFilter = item.GetComponent<MeshFilter>();
        }
    }
}
