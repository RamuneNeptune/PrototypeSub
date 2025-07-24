using System;
using PrototypeSubMod.MiscMonobehaviors.Materials;
using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.SubSystems;

public class ProtoWindowManager : MonoBehaviour
{
    private static readonly int ColorID = Shader.PropertyToID("_Color");
    
    [SerializeField] private ApplyCyclopsMaterial applyCyclopsMaterial;
    [SerializeField] private Renderer windowsRend;
    [SerializeField] private Color tintedColor;
    [SerializeField] private float minLerpDist;
    [SerializeField] private float maxLerpDist;

    private Color initialColor;
    private float sqrMinDist;
    private float sqrMaxDist;
    private bool initialized;
    
    private void Start()
    {
        applyCyclopsMaterial.onEditMaterial += _ => Init();
        sqrMinDist = minLerpDist * minLerpDist;
        sqrMaxDist = maxLerpDist * maxLerpDist;
    }

    private void Init()
    {
        initialColor = windowsRend.materials[1].GetColor(ColorID);
        initialized = true;
    }

    private void Update()
    {
        if (!initialized) return;
        
        float dist = Mathf.InverseLerp(sqrMinDist, sqrMaxDist, (Camera.main.transform.position - transform.position).sqrMagnitude);
        dist = Mathf.Clamp01(dist);
        var col = UnityEngine.Color.Lerp(initialColor, tintedColor, dist);

        var mats = windowsRend.materials;
        mats[1].SetColor(ColorID, col);
        windowsRend.materials = mats;
    }

    public float GetSqrMaxDistance() => sqrMaxDist;
}