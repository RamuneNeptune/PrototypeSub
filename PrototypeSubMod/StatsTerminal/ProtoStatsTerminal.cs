using UnityEngine;

namespace PrototypeSubMod.StatsTerminal;

internal class ProtoStatsTerminal : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private IStatistic[] statistics;

    private void Start()
    {
        statistics = GetComponents<IStatistic>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.Equals(Player.main.gameObject)) return;

        animator.SetBool("TerminalOpen", true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.Equals(Player.main.gameObject)) return;

        animator.SetBool("TerminalOpen", false);
    }

    private void Update()
    {
        foreach (var item in statistics)
        {
            item.UpdateStat();
        }
    }
}
