using DitzelGames.FastIK;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class IKFabricGizmos : MonoBehaviour
{
    [SerializeField] private FastIKFabric fabric;

    private void OnValidate()
    {
        if (!fabric) TryGetComponent(out fabric);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!fabric) return;

        var current = fabric.transform;
        for (int i = 0; i < fabric.ChainLength && current != null && current.parent != null; i++)
        {
            var scale = Vector3.Distance(current.position, current.parent.position) * 0.1f;
            Handles.matrix = Matrix4x4.TRS(current.position, Quaternion.FromToRotation(Vector3.up, current.parent.position - current.position), new Vector3(scale, Vector3.Distance(current.parent.position, current.position), scale));
            Handles.color = Color.green;
            Handles.DrawWireCube(Vector3.up * 0.5f, Vector3.one);
            current = current.parent;
        }
    }
#endif
}
