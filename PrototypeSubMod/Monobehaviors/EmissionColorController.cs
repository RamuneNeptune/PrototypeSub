using SubLibrary.Materials.Tags;
using SubLibrary.Monobehaviors;
using System.Collections.Generic;
using UnityEngine;

namespace PrototypeSubMod.Monobehaviors;

internal class EmissionColorController : PrefabModifier
{
    [SerializeField] private GameObject subRoot;
    [SerializeField] private float transitionSpeed;

    private Dictionary<Material, Color> trackedMaterials = new();
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

    public void SetTempColor(Color color)
    {
        tempColorActive = true;
        tempColor = color;
        currentTransitionTime = 0;
    }

    public void ClearTempColor()
    {
        tempColorActive = false;
        currentTransitionTime = 0;
    }
}
