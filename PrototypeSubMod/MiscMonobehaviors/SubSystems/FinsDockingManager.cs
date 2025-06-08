using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.SubSystems;

public class FinsDockingManager : MonoBehaviour
{
    private static readonly int DockingPrep = Animator.StringToHash("DockingPrep");
    private static readonly int DockingGrab = Animator.StringToHash("DockingGrab");

    [SerializeField] private Animator[] firstTwoFins;
    
    // These are called via ForwardMessageTrigger
    public void LaunchbayAreaEnter(GameObject nearby)
    {
        var vehicle = UWE.Utils.GetComponentInHierarchy<Vehicle>(nearby.gameObject);
        if (!vehicle || vehicle.docked) return;

        foreach (var animator in firstTwoFins)
        {
            animator.SetBool(DockingPrep, true);
        }
    }
    
    public void LaunchbayAreaExit(GameObject nearby)
    {
        var vehicle = UWE.Utils.GetComponentInHierarchy<Vehicle>(nearby.gameObject);
        if (!vehicle || vehicle.docked) return;
        
        foreach (var animator in firstTwoFins)
        {
            animator.SetBool(DockingPrep, false);
        }
    }

    public void StartGrabAnimation()
    {
        foreach (var animator in firstTwoFins)
        {
            animator.SetTrigger(DockingGrab);
        }
    }
}