using System.Collections;
using PrototypeSubMod.LightDistortionField;
using UnityEngine;
using UnityEngine.Events;

namespace PrototypeSubMod.SubTerminal;

internal class MoonpoolOccupiedHandler : MonoBehaviour, IProtoTreeEventListener
{
    public bool MoonpoolHasSub
    {
        get;
        private set;
    }

    public GameObject SubInMoonpool { get; private set; }

    public UnityEvent onHasSubChanged;
    [SerializeField] private ProtoBuildTerminal buildTerminal;
    [SerializeField] private BoxCollider moonpoolBounds;
    [SerializeField] private float maxDistanceFromMoonpool;

    private Bounds checkBounds;
    private bool occupiedLastCheck;
    
    private void CheckForSub()
    {
        bool foundSub = false;
        SubInMoonpool = null;

        foreach (var handler in CloakEffectHandler.EffectHandlers)
        {
            var subRoot = handler.GetComponentInParent<SubRoot>();
            if (checkBounds.Contains(subRoot.transform.position))
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

    public void OnProtoSerializeObjectTree(ProtobufSerializer serializer) { }

    public void OnProtoDeserializeObjectTree(ProtobufSerializer serializer)
    {
        UWE.CoroutineHost.StartCoroutine(Initialize());
    }

    private IEnumerator Initialize()
    {
        moonpoolBounds.enabled = true;
        yield return new WaitForEndOfFrame();
        checkBounds = new Bounds(moonpoolBounds.transform.position, moonpoolBounds.bounds.size);
        moonpoolBounds.enabled = false;
        
        CancelInvoke(nameof(CheckForSub));
        InvokeRepeating(nameof(CheckForSub), 0, 5f);
    }
}
