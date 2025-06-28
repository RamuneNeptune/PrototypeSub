using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.SubSystems;

public class ApplyVehicleLayer : MonoBehaviour
{
    private void Start()
    {
        foreach (var col in GetComponentsInChildren<Collider>(true))
        {
            col.gameObject.layer = LayerID.Vehicle;
        }
    }
}