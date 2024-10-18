using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PrototypeSubMod.Teleporter;

internal class TeleporterFXColorManager : MonoBehaviour
{
    [SerializeField] private Transform fxParent;

    private Dictionary<Renderer, Color> originalColors;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => fxParent.childCount > 0);

        foreach (var rend in fxParent.GetComponentsInChildren<Renderer>())
        {
            originalColors.Add(rend, rend.material.color);
        }
    }

    public void SetTempColor(Color color)
    {
        foreach (var rend in originalColors.Keys)
        {
            rend.material.color = color;
        }
    }

    public void ResetColor()
    {
        foreach (var kvp in originalColors)
        {
            kvp.Key.material.color = kvp.Value;
        }
    }
}
