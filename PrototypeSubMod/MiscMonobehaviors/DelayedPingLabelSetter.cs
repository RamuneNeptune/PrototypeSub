using System.Collections;
using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors;

internal class DelayedPingLabelSetter : MonoBehaviour
{
    public string translationKey;
    public PingInstance pingInstance;

    private void Awake()
    {
        if (TryGetComponent(out SignalPing ping))
        {
            ping.pos = transform.position;
            ping.descriptionKey = translationKey;
        }
    }

    private IEnumerator Start()
    {
        pingInstance.SetLabel(Language.main.Get(translationKey));
        yield return new WaitForEndOfFrame();

        pingInstance.SetColor(0);
    }
}
