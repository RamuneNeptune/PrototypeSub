using UnityEditor;
using UnityEngine;

public class FlipColliders : MonoBehaviour
{
    public Transform scalar;
    public Transform rotationHolder;
    public Transform collidersParent;
    public Transform newParent;
    public bool flip;

    private void OnDrawGizmos()
    {
        if (!transform || !collidersParent) return;

        if (!flip) return;

        flip = false;

        EditorGUI.BeginChangeCheck();
        foreach (Transform t in collidersParent)
        {
            var copy = Instantiate(t.gameObject);
            copy.transform.position = scalar.InverseTransformPoint(copy.transform.position);

            var forwardReflect = Vector3.Reflect(copy.transform.rotation * Vector3.forward, rotationHolder.forward);
            var upwardReflect = Vector3.Reflect(copy.transform.rotation * Vector3.up, rotationHolder.forward);

            copy.transform.rotation = Quaternion.LookRotation(forwardReflect, upwardReflect);
            copy.transform.SetParent(newParent);
        }
        EditorGUI.EndChangeCheck();
    }
}
