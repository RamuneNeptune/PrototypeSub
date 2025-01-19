using UnityEngine;

[ExecuteInEditMode]
public class ApplyMaterial : MonoBehaviour
{
    [SerializeField] private Material material;
    [SerializeField] private bool remove;

    private void Update()
    {
        if (!remove) return;
        remove = false;
        var rends = GetComponentsInChildren<MeshRenderer>(true);
        foreach (var rend in rends)
        {
            rend.material = material;
        }
    }
}
