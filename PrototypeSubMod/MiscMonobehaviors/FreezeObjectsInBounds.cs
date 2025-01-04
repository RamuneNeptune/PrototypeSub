using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors;

internal class FreezeObjectsInBounds : MonoBehaviour
{
    [SerializeField] private Collider collider;
    [SerializeField] private float updateDuration = 2f;

    private float currentUpdateDuration;

    private void Start()
    {
        collider.enabled = false;
    }

    private void Update()
    {
        if (currentUpdateDuration < updateDuration)
        {
            currentUpdateDuration += Time.deltaTime;
            FreezeObjects();
        }
    }

    private void FreezeObjects()
    {
        int count = UWE.Utils.OverlapBoxIntoSharedBuffer(collider.bounds.center, collider.bounds.extents, collider.transform.rotation);
        for (int i = 0; i < count; i++)
        {
            var col = UWE.Utils.sharedColliderBuffer[i];
            var rigidBody = col.GetComponent<Rigidbody>();
            if (!rigidBody) continue;

            UWE.Utils.SetIsKinematic(rigidBody, true);
        }
    }
}
