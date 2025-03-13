using System;
using UnityEngine;

namespace PrototypeSubMod.StatsTerminal;

internal class ProtoStatsTerminal : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private BehaviourLOD lod;
    [SerializeField] private float intermittentUpdateInterval = 2f;

    private event Action onUpdate;
    private event Action onUpdateIntermittent;

    private void Start()
    {
        var statistics = GetComponents<IStatistic>();
        InvokeRepeating(nameof(UpdateIntermittent), 0, intermittentUpdateInterval);

        foreach (var item in statistics)
        {
            onUpdate += item.UpdateStat;
            onUpdateIntermittent += item.UpdateStatIntermittent;
        }
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
        if ((transform.position - Player.main.transform.position).sqrMagnitude > 50) return;

        onUpdate?.Invoke();
    }

    private void UpdateIntermittent()
    {
        onUpdateIntermittent?.Invoke();
    }
}
