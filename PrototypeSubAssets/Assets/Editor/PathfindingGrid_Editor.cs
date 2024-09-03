using System.Reflection;
using UnityEditor;
using UnityEngine;
using PrototypeSubMod.Pathfinding;
using PrototypeSubMod.Pathfinding.SaveSystem;

[CustomEditor(typeof(PathfindingGrid))]
public class PathfindingGrid_Editor : Editor
{
    private MethodInfo initializeMethod;
    private MethodInfo createGridMethod;
    private FieldInfo gridInfo;

    private void Initialize()
    {
        if (initializeMethod != null && createGridMethod != null && gridInfo != null) return;

        initializeMethod = typeof(PathfindingGrid).GetMethod("Initialize", BindingFlags.NonPublic | BindingFlags.Instance);
        createGridMethod = typeof(PathfindingGrid).GetMethod("CreateGrid", BindingFlags.NonPublic | BindingFlags.Instance);
        gridInfo = typeof(PathfindingGrid).GetField("grid", BindingFlags.NonPublic | BindingFlags.Instance);
    }

    public override void OnInspectorGUI()
    {
        Initialize();

        base.OnInspectorGUI();
        var grid = (PathfindingGrid)target;

        if (GUILayout.Button("Generate and Save to Assets"))
        {
            initializeMethod.Invoke(grid, null);
            createGridMethod.Invoke(grid, null);

            var wrappedGenPos = new SerializableV3Wrapper(grid.GetPositionAtGridGen());
            var wrappedGenRot = new SerializableQuaternionWrapper(grid.GetRotationAtGridGen());

            var saveData = new GridSaveData((GridNode[,,])gridInfo.GetValue(grid), wrappedGenPos, wrappedGenRot);
            SaveManager.SerializeObject(saveData, $"Assets\\SaveGrid.grid.bytes");
            AssetDatabase.Refresh();
        }
    }
}
