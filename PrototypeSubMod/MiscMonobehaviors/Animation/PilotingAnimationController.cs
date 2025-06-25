using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.Animation;

public class PilotingAnimationController : MonoBehaviour
{
    private static readonly int HorizontalVal = Animator.StringToHash("horizontalVal");
    private static readonly int VerticalVal = Animator.StringToHash("verticalVal");
    private static readonly int ForwardVal = Animator.StringToHash("forwardVal");
    
    [SerializeField] private SubControl subControl;
    [SerializeField] private Animator animator;

    private float currentForwardVal;
    
    private void LateUpdate()
    {
        float xVal = (subControl.steeringWheelYaw + 90) / 180f;
        float yVal = (subControl.steeringWheelPitch + 90) / 180f;

        currentForwardVal = Mathf.Lerp(currentForwardVal, GameInput.GetMoveDirection().z,
            Time.deltaTime * subControl.steeringReponsiveness);

        float zVal = (currentForwardVal + 1) * 0.5f;
        animator.SetFloat(HorizontalVal, xVal);
        animator.SetFloat(VerticalVal, yVal);
        animator.SetFloat(ForwardVal, zVal);
    }
}