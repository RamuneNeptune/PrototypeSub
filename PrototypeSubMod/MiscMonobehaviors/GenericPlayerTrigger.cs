using UnityEngine;
using UnityEngine.Events;

namespace PrototypeSubMod.MiscMonobehaviors;

[RequireComponent(typeof(Collider))]
internal class GenericPlayerTrigger : MonoBehaviour
{
    [SerializeField] private UnityEvent onTriggerEnter;
    [SerializeField] private UnityEvent onTriggerExit;

    private void OnTriggerEnter(Collider col)
    {
        if (!col.gameObject.Equals(Player.main.gameObject)) return;

        onTriggerEnter.Invoke();
    }

    private void OnTriggerExit(Collider col)
    {
        if (!col.gameObject.Equals(Player.main.gameObject)) return;

        onTriggerExit.Invoke();
    }
}
