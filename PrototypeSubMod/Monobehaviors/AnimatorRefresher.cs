using UnityEngine;

namespace PrototypeSubMod.Monobehaviors;

internal class AnimatorRefresher : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private void LateUpdate()
    {
        animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
    }
}
