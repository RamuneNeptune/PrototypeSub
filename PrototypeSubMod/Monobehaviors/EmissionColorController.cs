using PrototypeSubMod.Extensions;
using SubLibrary.Materials.Tags;
using SubLibrary.Monobehaviors;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PrototypeSubMod.Monobehaviors;

internal class EmissionColorController : PrefabModifier
{
    [SerializeField] private GameObject subRoot;
    [SerializeField] private float transitionSpeed;

    private Dictionary<Material, Color> trackedMaterials = new();
    private List<EmissionRegistrarData> overrideColorData = new();
    private bool initialized;

    private Color tempColor;
    private bool tempColorActive;
    private float transitionTimeOut;
    private float currentTransitionTime;

    private void Start()
    {
        transitionTimeOut = (1 / transitionSpeed) * 8f;
        currentTransitionTime = transitionTimeOut;

        foreach (var rend in subRoot.GetComponentsInChildren<Renderer>(true))
        {
            if (rend.GetComponent<EmissionControllerExempt>()) continue;

            foreach (var mat in rend.materials)
            {
                if (!mat.IsKeywordEnabled("MARMO_EMISSION")) continue;

                trackedMaterials.Add(mat, mat.GetColor("_GlowColor"));
            }
        }

        initialized = true;
    }

    private void Update()
    {
        if (!initialized) return;

        if (currentTransitionTime < transitionTimeOut)
        {
            currentTransitionTime += Time.deltaTime;
        }
        else
        {
            return;
        }

        foreach (var material in trackedMaterials.Keys)
        {
            Color targetCol = tempColorActive ? tempColor : trackedMaterials[material];
            Color currentCol = Color.Lerp(material.GetColor("_GlowColor"), targetCol, transitionSpeed * Time.deltaTime);
            material.SetColor("_GlowColor", currentCol);
        }
    }

    public void RegisterTempColor(EmissionRegistrarData registerData)
    {
        overrideColorData.Add(registerData);
        tempColorActive = true;
        currentTransitionTime = 0;

        UpdateTempColor();
    }

    public void RemoveTempColor(Component component)
    {
        var data = overrideColorData.FirstOrDefault(i => i.owner == component);
        if (data.Equals(default)) return;

        overrideColorData.Remove(data);
        tempColorActive = overrideColorData.Count > 0;
        currentTransitionTime = 0;

        UpdateTempColor();
    }

    private void UpdateTempColor()
    {
        int greatestPriority = int.MinValue;
        foreach (var data in overrideColorData)
        {
            if (data.priority > greatestPriority)
            {
                greatestPriority = data.priority;
                tempColor = data.overrideColor;
            }
        }
    }

    public struct EmissionRegistrarData
    {
        public Component owner;
        public Color overrideColor;
        public int priority;

        public EmissionRegistrarData(Component owner, Color overrideColor, int priority = 10)
        {
            this.owner = owner;
            this.overrideColor = overrideColor;
            this.priority = priority;
        }
    }
}
