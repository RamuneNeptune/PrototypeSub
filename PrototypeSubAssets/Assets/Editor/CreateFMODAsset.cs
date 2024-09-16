using System.IO;
using UnityEditor;
using UnityEngine;

public class CreateFMODAsset
{
    [MenuItem("Assets/Create/Subnautica/Create FMOD Asset (Deprecated)")]
    public static void CreateAsset()
    {
        FMODAsset asset = ScriptableObject.CreateInstance<FMODAsset>();

        var obj = Selection.activeObject;
        string path = obj ? AssetDatabase.GetAssetPath(obj) : "Assets";

        if(path.Contains("."))
        {
            path = $"Assets/{Directory.GetParent(path).Name}";
        }

        AssetDatabase.CreateAsset(asset, $"{path}/CustomFMODAsset.asset");
        AssetDatabase.SaveAssets();

        Selection.activeObject = asset;
    }
}
