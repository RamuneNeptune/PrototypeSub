using UnityEngine;

[ExecuteInEditMode]
public class RemoveMeshes : MonoBehaviour
{
    [SerializeField] private bool remove;

    private void Update()
    {
        if (!remove) return;
        remove = false;
        var rends = GetComponentsInChildren<MeshRenderer>(true);
        for (int i = rends.Length - 1; i >= 0; i--)
        {
            var filter = rends[i].GetComponent<MeshFilter>();
            DestroyImmediate(filter);
            DestroyImmediate(rends[i]);
        }
    }
}
