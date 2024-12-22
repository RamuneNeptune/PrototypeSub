using PrototypeSubMod.PowerSystem;
using SubLibrary.SaveData;
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

    public GameObject SubInMoonpool { get; private set; }

    [SerializeField] private UnityEvent onHasSubChanged;
    [SerializeField] private ProtoBuildTerminal buildTerminal;
    [SerializeField] private Collider moonpoolBounds;
    [SerializeField] private float maxDistanceFromMoonpool;

    private bool occupiedLastCheck;

    private void Start()
    {
        CancelInvoke(nameof(CheckForSub));
        InvokeRepeating(nameof(CheckForSub), 0, 5f);
    }

    private void CheckForSub()
    {
        bool foundSub = false;
        SubInMoonpool = null;

        int count = UWE.Utils.OverlapBoxIntoSharedBuffer(moonpoolBounds.transform.position, moonpoolBounds.bounds.extents / 2, moonpoolBounds.transform.rotation);
        for (int i = 0; i < count; i++)
        {
            var collider = UWE.Utils.sharedColliderBuffer[i];
            var serializationManager = collider.GetComponentInParent<SubSerializationManager>();

            if (serializationManager == null) continue;

            var powerSystem = serializationManager.GetComponentInChildren<PrototypePowerSystem>();

            if (powerSystem == null) continue;

            SubInMoonpool = serializationManager.gameObject;
            foundSub = true;
            break;
        }

        MoonpoolHasSub = foundSub;

        if (occupiedLastCheck != MoonpoolHasSub)
        {
            onHasSubChanged?.Invoke();
        }

        occupiedLastCheck = MoonpoolHasSub;
    }
}
