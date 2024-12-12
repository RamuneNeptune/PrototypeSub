using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors;

internal class ColliderArea : MonoBehaviour
{
    [SerializeField] private GameObject[] colliders;

    private void Start()
    {
        foreach (var obj in colliders)
        {
            obj.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col != Player.mainCollider) return;

        foreach (var obj in colliders)
        {
            obj.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col != Player.mainCollider) return;

        foreach (var obj in colliders)
        {
            obj.SetActive(false);
        }
    }
}
