using UnityEngine;
using UnityEngine.Events;

namespace PrototypeSubMod.SubTerminal;

internal class MoonpoolOccupiedHandler : MonoBehaviour
{
    public bool MoonpoolHasSub
    {
        get;
        private set;
    }

    private bool occupiedLastCheck;

    [SerializeField] private UnityEvent onHasSubChanged;
    [SerializeField] private ProtoBuildTerminal buildTerminal;
    [SerializeField] private float maxDistanceFromMoonpool;

    private void Start()
    {
        CancelInvoke(nameof(CheckForSub));
        InvokeRepeating(nameof(CheckForSub), 0, 5f);
    }

    private void CheckForSub()
    {
        if (buildTerminal.PrototypeSub == null)
        {
            MoonpoolHasSub = false;
            return;
        }

        Vector3 distance = transform.position - buildTerminal.PrototypeSub.transform.position;

        MoonpoolHasSub = buildTerminal.HasBuiltProtoSub && distance.sqrMagnitude < (maxDistanceFromMoonpool * maxDistanceFromMoonpool);

        if (occupiedLastCheck != MoonpoolHasSub)
        {
            onHasSubChanged?.Invoke();
        }

        occupiedLastCheck = MoonpoolHasSub;
    }
}
