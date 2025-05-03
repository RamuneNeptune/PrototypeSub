using PrototypeSubMod.Pathfinding;
using UnityEngine;

public class Test : MonoBehaviour
{
    public bool increaseFPS;
    public GameObject staticBatchRoot;

    private void OnDrawGizmos()
    {
        if (!increaseFPS) return;
        increaseFPS = false;

        StaticBatchingUtility.Combine(staticBatchRoot);
        Debug.Log("Combined meshes");
    }
}
