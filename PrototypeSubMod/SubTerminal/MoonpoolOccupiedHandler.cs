using System.Collections;
using PrototypeSubMod.LightDistortionField;
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

    public UnityEvent onHasSubChanged;
    [SerializeField] private BoxCollider moonpoolBounds;
    
    private bool occupiedLastCheck;

    private void Start()
    {
        Initialize();
    }
    
    public void CheckForSub()
    {
        bool foundSub = false;
        SubInMoonpool = null;

        foreach (var handler in CloakEffectHandler.EffectHandlers)
        {
            var subRoot = handler.GetComponentInParent<SubRoot>();
            if (moonpoolBounds.bounds.Contains(subRoot.transform.position))
            {
                foundSub = true;
                SubInMoonpool = subRoot.gameObject;
                break;
            }
        }

        MoonpoolHasSub = foundSub;

        if (occupiedLastCheck != MoonpoolHasSub)
        {
            onHasSubChanged?.Invoke();
        }

        occupiedLastCheck = MoonpoolHasSub;
    }

    private void Initialize()
    {
        CancelInvoke(nameof(CheckForSub));
        InvokeRepeating(nameof(CheckForSub), 0, 5f);
    }
}
