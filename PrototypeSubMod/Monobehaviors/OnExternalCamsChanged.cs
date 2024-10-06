using UnityEngine;
using UnityEngine.Events;

namespace PrototypeSubMod.Monobehaviors;

[RequireComponent(typeof(CyclopsExternalCams))]
internal class OnExternalCamsChanged : MonoBehaviour
{
    [SerializeField] private UnityEvent onCamsEnabled;
    [SerializeField] private UnityEvent onCamsDisabled;

    private CyclopsExternalCams externalCams;
    private bool enabledLastFrame;

    private void Start()
    {
        externalCams = GetComponent<CyclopsExternalCams>();
    }

    private void LateUpdate()
    {
        if (externalCams.active != enabledLastFrame)
        {
            if (externalCams.active) onCamsEnabled?.Invoke();
            if (!externalCams.active) onCamsDisabled?.Invoke();
        }

        enabledLastFrame = externalCams.active;
    }
}
