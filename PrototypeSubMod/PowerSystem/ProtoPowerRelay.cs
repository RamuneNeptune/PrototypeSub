using UnityEngine;

namespace PrototypeSubMod.PowerSystem;

public class ProtoPowerRelay : MonoBehaviour
{
    private static readonly int PylonActive = Animator.StringToHash("PylonActive");
    [SerializeField] private Animator animator;
    
    public void SetRelayActive(bool active)
    {
        animator.SetBool(PylonActive, active);
    }

    public void SetSourceType(TechType techType)
    {
        
    }
}