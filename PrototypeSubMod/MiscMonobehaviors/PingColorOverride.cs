using System.Collections;
using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors;

internal class PingColorOverride : MonoBehaviour
{
    public Color overrideColor;

    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        GetComponent<PingInstance>().SetColor(0);
    }
}
