using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.SubSystems;

internal class WatergateVehicleBlocker : MonoBehaviour
{
    [SerializeField] private BehaviourLOD lod;
    [SerializeField] private SubRoot subRoot;
    [SerializeField] private Collider[] colliders;
    [SerializeField] private float timeBetweenChecks = 1;

    private void Start()
    {
        InvokeRepeating(nameof(CheckIfPlayerInVehicle), 0, timeBetweenChecks);
    }

    private void CheckIfPlayerInVehicle()
    {
        if (!lod.IsFull()) return;

        bool inVehicle = Player.main.currentMountedVehicle != null;
        bool inSub = Player.main.currentSub != null && Player.main.currentSub != subRoot;

        foreach (var item in colliders)
        {
            item.isTrigger = !(inVehicle || inSub);
        }
    }
}
