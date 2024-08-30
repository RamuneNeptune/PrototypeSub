using UnityEditor;
using ThunderKit.Core.Manifests.Datums;
using UnityEngine;
using System;
using System.Collections;

[CustomEditor(typeof(AssetBundleDefinitions))]
public class AssetBundleSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var definitions = (AssetBundleDefinitions)target;

        if(GUILayout.Button("Sort Assets"))
        {
            foreach (var bundle in definitions.assetBundles)
            {
                Array.Sort(bundle.assets, new ObjectComparer());
            }
        }

        if(GUILayout.Button("Reverse List"))
        {
            foreach (var bundle in definitions.assetBundles)
            {
                Array.Reverse(bundle.assets);
            }
        }
    }
}

public class ObjectComparer : IComparer
{
    public int Compare(object x, object y)
    {
        return x.GetType().ToString().CompareTo(y.GetType().ToString());
    }
}
