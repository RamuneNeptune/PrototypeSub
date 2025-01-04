using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors;

internal class FreezeObjectsInBounds : MonoBehaviour
{
    [SerializeField] private Collider collider;
    [SerializeField] private float updateDuration = 2f;

    private float currentUpdateDuration;

    private void Start()
    {
        collider.gameObject.SetActive(false);
        FreezeObjects();
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
            var rigidBody = col.GetComponentInParent<Rigidbody>();
            if (!rigidBody) continue;

            ErrorMessage.AddError($"Freezing {rigidBody}");
            UWE.Utils.SetIsKinematic(rigidBody, true);
            Destroy(col);

            var worldForces = col.GetComponentInParent<WorldForces>();
            if (worldForces) Destroy(worldForces);
        }
    }
}
