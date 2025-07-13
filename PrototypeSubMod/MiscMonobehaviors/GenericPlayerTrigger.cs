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
        if (col.isTrigger) return;
        
        bool isPlayerCol = col.gameObject.Equals(Player.main.gameObject);
        var vehicle = col.GetComponentInParent<Vehicle>();
        bool isPlayerVehicle = vehicle && Player.main.currentMountedVehicle == vehicle;
        
        if (!isPlayerCol && !isPlayerVehicle) return;

        onTriggerEnter.Invoke();
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.isTrigger) return;
        
        bool isPlayerCol = col.gameObject.Equals(Player.main.gameObject);
        bool isPlayerVehicle = col.TryGetComponent(out Vehicle vehicle) && Player.main.currentMountedVehicle == vehicle;
        if (!isPlayerCol && !isPlayerVehicle) return;

        onTriggerExit.Invoke();
    }
}
