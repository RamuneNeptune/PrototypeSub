using UnityEditor;
using UnityEngine;

public class CreateFMODAsset
{
    [MenuItem("Assets/Create/Subnautica/Create FMOD Asset")]
    public static void CreateAsset()
    {
        FMODAsset asset = ScriptableObject.CreateInstance<FMODAsset>();

        AssetDatabase.CreateAsset(asset, "Assets/NewFMODAsset.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}
