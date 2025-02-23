using UnityEngine;

public class Test : MonoBehaviour
{
    public bool enabled;

    private void OnDrawGizmos()
    {
        if (!enabled) return;

        enabled = false;
        foreach (var r in GetComponentsInChildren<Renderer>())
        {
            Debug.Log($"{r.sharedMaterial.renderQueue} | {r.sharedMaterial.shader.renderQueue}");
            r.sharedMaterial.renderQueue = r.material.shader.renderQueue;
        }
    }
}
