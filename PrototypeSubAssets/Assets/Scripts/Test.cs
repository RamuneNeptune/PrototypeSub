using PrototypeSubMod.Pathfinding;
using UnityEngine;

public class Test : MonoBehaviour
{
    public bool updatePaths;

    private void OnDrawGizmos()
    {
        if (!updatePaths) return;

        updatePaths = false;
        foreach (var t in GetComponentsInChildren<PathfindingObject>())
        {
            t.UpdatePath();
        }
    }
}
