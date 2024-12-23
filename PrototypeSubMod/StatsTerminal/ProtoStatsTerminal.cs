using UnityEngine;

namespace PrototypeSubMod.StatsTerminal;

internal class ProtoStatsTerminal : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private void OnTriggerEnter(Collider other)
    {
        if (other != Player.mainCollider) return;

        animator.SetBool("TerminalOpen", true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other != Player.mainCollider) return;

        animator.SetBool("TerminalOpen", true);
    }
}
