using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors;

internal class DelayedPingLabelSetter : MonoBehaviour
{
    public string translationKey;
    public PingInstance pingInstance;

    private void Start()
    {
        pingInstance.SetLabel(Language.main.Get(translationKey));
    }
}
