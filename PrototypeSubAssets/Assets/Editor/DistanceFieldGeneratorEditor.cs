using UnityEditor;
using SubLibrary.Utilities;
using UnityEngine;

[CustomEditor(typeof(GenerateDistanceField))]
public class DistanceFieldGeneratorEditor : Editor
{
    private const int MAX_LABEL_WIDTH = 10;

    public override void OnInspectorGUI()
    {
        GenerateDistanceField generator = target as GenerateDistanceField;

        EditorGUILayout.LabelField("Config values");

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Bounds", GUILayout.MaxWidth(50));

        EditorGUILayout.LabelField("X", GUILayout.MaxWidth(10));
        float x1 = EditorGUILayout.FloatField(generator.bounds.center.x);
        EditorGUILayout.LabelField("Y", GUILayout.MaxWidth(10));
        float y1 = EditorGUILayout.FloatField(generator.bounds.center.y);
        EditorGUILayout.LabelField("Z", GUILayout.MaxWidth(10));
        float z1 = EditorGUILayout.FloatField(generator.bounds.center.z);

        generator.bounds.center = new Vector3(x1, y1, z1);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Extents", GUILayout.MaxWidth(50));

        EditorGUILayout.LabelField("X", GUILayout.MaxWidth(10));
        float x2 = EditorGUILayout.FloatField(generator.bounds.extents.x);
        EditorGUILayout.LabelField("Y", GUILayout.MaxWidth(10));
        float y2 = EditorGUILayout.FloatField(generator.bounds.extents.y);
        EditorGUILayout.LabelField("Z", GUILayout.MaxWidth(10));
        float z2 = EditorGUILayout.FloatField(generator.bounds.extents.z);

        generator.bounds.extents = new Vector3(x2, y2, z2);
        EditorGUILayout.EndHorizontal();

        generator.distanceField = EditorGUILayout.ObjectField("Distance field" ,generator.distanceField, typeof(DistanceField), false) as DistanceField;
        generator.pixelsPerUnit = EditorGUILayout.FloatField("Pixels per unit", generator.pixelsPerUnit);

        if(GUILayout.Button("Generate texture"))
        {
            generator.GenerateTexture();
        }

        if (GUILayout.Button("Toggle preview"))
        {
            generator.visualizeInEditor = !generator.visualizeInEditor;
        }

        if (!generator.visualizeInEditor) return;

        generator.crossSectionVisualizationDepth = EditorGUILayout.Slider("Cross Section Depth", generator.crossSectionVisualizationDepth, 0, 1);
        generator.crossSectionAxis = (GenerateDistanceField.Axis)EditorGUILayout.EnumFlagsField("Cross Section Axis", generator.crossSectionAxis);
        generator.inverseVisualization = EditorGUILayout.Toggle("Invert Visualization", generator.inverseVisualization);
    }
}
