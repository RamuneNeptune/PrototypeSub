using System.Collections;
using UnityEngine;

namespace PrototypeSubMod.Monobehaviors;

internal class AtmospherePriorityEnsurer : MonoBehaviour
{
    public int priority;

    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        var volume = GetComponent<AtmosphereVolume>();
        volume.priority = priority;
        volume.settings.priority = priority;
    }
}
