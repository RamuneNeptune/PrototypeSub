using PrototypeSubMod.PowerSystem;
using UnityEngine;
using UnityEngine.Events;

namespace PrototypeSubMod.SubTerminal;

internal class MoonpoolOccupiedHandler : MonoBehaviour
{
    public bool MoonpoolHasSub { get; private set; }

    [SerializeField] private UnityEvent onHasSubChanged;

    private void OnTriggerStay(Collider col)
    {
        var root = UWE.Utils.GetEntityRoot(col.gameObject);

        if (root == null) return;

        if (root.GetComponentInChildren<PrototypePowerSystem>() != null && !MoonpoolHasSub)
        {
            MoonpoolHasSub = true;
            onHasSubChanged?.Invoke();
        }
    }

    private void OnTriggerExit(Collider col)
    {
        var root = UWE.Utils.GetEntityRoot(col.gameObject);

        if (root == null) return;

        if (root.GetComponentInChildren<PrototypePowerSystem>() != null && MoonpoolHasSub)
        {
            MoonpoolHasSub = false;
            onHasSubChanged?.Invoke();
        }
    }
}
