using PrototypeSubMod.MiscMonobehaviors.Materials;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(MaterialData))]
public class MaterialData_Editor : PropertyDrawer
{
    private SerializedProperty name;
    private SerializedProperty propertyName;
    private SerializedProperty childPath;
    private SerializedProperty materialIndex;
    private SerializedProperty propertyType;
    private SerializedProperty floatValue;
    private SerializedProperty vectorValue;
    private SerializedProperty textureValue;
    private SerializedProperty colorValue;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 0;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        propertyName = property.FindPropertyRelative("propertyName");
        childPath = property.FindPropertyRelative("childPath");
        materialIndex = property.FindPropertyRelative("materialIndex");
        propertyType = property.FindPropertyRelative("type");
        floatValue = property.FindPropertyRelative("floatValue");
        vectorValue = property.FindPropertyRelative("vectorValue");
        textureValue = property.FindPropertyRelative("textureValue");
        colorValue = property.FindPropertyRelative("colorValue");

        propertyName.stringValue = EditorGUILayout.TextField("Property Name", propertyName.stringValue);
        childPath.stringValue = EditorGUILayout.TextField("Child Path", childPath.stringValue);
        materialIndex.intValue = EditorGUILayout.IntField("Material Index", materialIndex.intValue);
        var enumVal = (MaterialData.PropertyType)EditorGUILayout.EnumPopup("Property Type", (MaterialData.PropertyType)propertyType.enumValueIndex);
        propertyType.enumValueIndex = (int)enumVal;

        switch (enumVal)
        {
            case MaterialData.PropertyType.Float:
                floatValue.floatValue = EditorGUILayout.FloatField("Value", floatValue.floatValue);
                break;
            case MaterialData.PropertyType.Vector:
                vectorValue.vector4Value = EditorGUILayout.Vector4Field("Value", vectorValue.vector4Value);
                break;
            case MaterialData.PropertyType.Texture:
                textureValue.objectReferenceValue = EditorGUILayout.ObjectField("Value", textureValue.objectReferenceValue, typeof(Texture), false);
                break;
            case MaterialData.PropertyType.Color:
                colorValue.colorValue = EditorGUILayout.ColorField("Value", colorValue.colorValue);
                break;
        }

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
    }
}
