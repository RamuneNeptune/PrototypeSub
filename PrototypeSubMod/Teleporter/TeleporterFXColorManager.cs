using System.Collections;
using System.Collections.Generic;
using PrototypeSubMod.Patches;
using UnityEngine;

namespace PrototypeSubMod.Teleporter;

internal class TeleporterFXColorManager : MonoBehaviour
{
    [SerializeField] private Transform fxParent;

    private Renderer rend;
    private Light light;
    private Color originalCol;

    private Dictionary<Component, TempColor> tempColors = new();

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => fxParent.childCount > 0);
        yield return new WaitForEndOfFrame();

        rend = fxParent.GetComponentInChildren<Renderer>(true);
        light = fxParent.GetComponentInChildren<Light>(true);
        originalCol = rend.material.GetColor("_ColorOuter");
    }

    private void Update()
    {
        if (rend == null) return;

        Color color = originalCol;
        int maxPriority = int.MinValue;
        foreach (var tempColor in tempColors.Values)
        {
            if (tempColor.priority > maxPriority)
            {
                color = tempColor.color;
                maxPriority = tempColor.priority;
            }
        }
        
        Color col = Color.Lerp(rend.material.GetColor("_ColorOuter"), color, Time.deltaTime);
        rend.material.SetColor("_ColorOuter", col);
        light.color = col;
    }

    public void AddTempColor(Component owner, TempColor color)
    {
        if (tempColors.ContainsKey(owner))
        {
            tempColors[owner] = color;
            return;
        }
        
        tempColors.Add(owner, color);
    }

    public void RemoveTempColor(Component owner)
    {
        tempColors.Remove(owner);
    }

    public class TempColor
    {
        public Color color;
        public int priority;

        public TempColor(Color color, int priority = 1)
        {
            color = color;
            this.priority = priority;
        }
    }
}
