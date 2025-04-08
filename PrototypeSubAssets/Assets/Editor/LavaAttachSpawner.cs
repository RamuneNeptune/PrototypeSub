using UnityEditor;
using UnityEngine;

public class LavaAttachSpawner : EditorWindow
{
    private static bool active;
    private static Transform parent;
    private static Material material;

    [MenuItem("Tools/Lava larva attach points")]
    private static void Init()
    {
        var window = (LavaAttachSpawner)EditorWindow.GetWindow(typeof(LavaAttachSpawner));
        window.Show();
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
    }

    private void OnEnable() => SceneView.duringSceneGui += OnSceneGUI;
    private void OnDisable() => SceneView.duringSceneGui -= OnSceneGUI;

    private void OnSceneGUI(SceneView view)
    {
        if (!active) return;

        if (Event.current.type == EventType.MouseDown && parent)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

            if (Physics.Raycast(ray, out var hitInfo))
            {
                var point = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                point.transform.position = hitInfo.point + hitInfo.normal * 0.25f;
                point.transform.forward = -hitInfo.normal;
                point.transform.localScale = Vector3.one * 0.2f;
                point.transform.SetParent(parent);
                DestroyImmediate(point.GetComponent<Collider>());

                point.name = $"LavaLarvaAttachPoint ({parent.childCount})";

                if (material != null)
                {
                    point.GetComponent<Renderer>().material = material;
                }
            }
        }
        
        Event.current.Use();
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Enable Raycasting"))
        {
            active = !active;
        }

        GUILayout.Label("Active = " + active);
        parent = (Transform)EditorGUILayout.ObjectField(parent, typeof(Transform), true);
        material = (Material)EditorGUILayout.ObjectField(material, typeof(Material), false);
    }
}
