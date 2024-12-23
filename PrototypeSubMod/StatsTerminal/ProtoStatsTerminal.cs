using UnityEngine;

namespace PrototypeSubMod.StatsTerminal;

internal class ProtoStatsTerminal : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private BehaviourLOD lod;
    [SerializeField] private float intermittentUpdateInterval = 2f;

    private IStatistic[] statistics;
    private float currentIntervalTime;

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
        if (!lod.IsFull()) return;

        foreach (var item in statistics)
        {
            item.UpdateStat();
        }

        if (currentIntervalTime < intermittentUpdateInterval)
        {
            currentIntervalTime += Time.deltaTime;
        }
        else
        {
            currentIntervalTime = 0;
            foreach (var item in statistics)
            {
                item.UpdateStatIntermittent();
            }
        }
    }
}
