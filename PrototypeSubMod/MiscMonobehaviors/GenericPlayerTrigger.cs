using UnityEngine;
using UnityEngine.Events;

namespace PrototypeSubMod.MiscMonobehaviors;

[RequireComponent(typeof(Collider))]
internal class GenericPlayerTrigger : MonoBehaviour
{
    [SerializeField] private UnityEvent onTriggerEnter;
    [SerializeField] private UnityEvent onTriggerExit;

    private void OnTriggerEnter(Collider other)
    {
        if (other != Player.mainCollider) return;

        onTriggerEnter?.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other != Player.mainCollider) return;

        onTriggerExit?.Invoke();
    }
}
