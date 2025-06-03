using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.Animation;

public class SetAnimatorBool : MonoBehaviour
{
    [SerializeField] private Animator animator;
    
    private string boolName;
    
    public void PreloadBoolName(string boolName)
    {
        this.boolName = boolName;
    }

    public void SetBoolActive(bool active)
    {
        animator.SetBool(boolName, active);
    }
}