using System.Collections;
using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.Materials;

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
