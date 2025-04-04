using UnityEngine;

public class FlipColliders : MonoBehaviour
{
    public Transform collidersParent;
    public Transform newParent;
    public bool flip;

    private void OnDrawGizmos()
    {
        if (!transform || !collidersParent) return;

        if (!flip) return;

        flip = false;
        
        foreach (Transform t in collidersParent)
        {
            var copy = Instantiate(t.gameObject);
            copy.transform.position = Vector3.Reflect(copy.transform.position, transform.forward);

            var forwardReflect = Vector3.Reflect(copy.transform.rotation * Vector3.forward, transform.forward);
            var upwardReflect = Vector3.Reflect(copy.transform.rotation * Vector3.up, transform.forward);

            copy.transform.rotation = Quaternion.LookRotation(forwardReflect, upwardReflect);
            copy.transform.SetParent(newParent);
        }
    }
}
