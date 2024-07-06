using UnityEditor;
using UnityEngine;

public class CreateDistanceField
{
    [MenuItem("Assets/Create/Subnautica/Create Distance Field")]
    public static void CreateAsset()
    {
        DistanceField asset = ScriptableObject.CreateInstance<DistanceField>();

        AssetDatabase.CreateAsset(asset, "Assets/NewDistanceField.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}
