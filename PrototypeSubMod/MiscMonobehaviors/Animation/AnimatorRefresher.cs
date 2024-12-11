using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.Animation;

internal class AnimatorRefresher : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private void LateUpdate()
    {
        animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
    }
}
