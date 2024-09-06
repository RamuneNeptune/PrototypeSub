using PrototypeSubMod.Pathfinding;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PathfindingObject))]
public class PathfindingObject_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var pathfindingObj = (PathfindingObject)target;
        if(GUILayout.Button("Update Path"))
        {
            pathfindingObj.UpdatePath();
        }
    }
}
