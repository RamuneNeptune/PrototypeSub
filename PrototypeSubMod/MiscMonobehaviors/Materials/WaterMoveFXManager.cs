using System;
using System.Collections.Generic;
using PrototypeSubMod.MiscMonobehaviors.PrefabRetrievers;
using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.Materials;

public class WaterMoveFXManager : MonoBehaviour
{
    [SerializeField] private SpawnPrefabAtRuntime prefabSpawner;
    [SerializeField] private float transitionSpeed;

    private Dictionary<Component, ColorData> colorDatas = new();

    private ColorData activeColorData;
    private Material matInner;
    private Material matOuter;
    
    private void Start()
    {
        prefabSpawner.onEditMaterial += OnObjectSpawned;
    }

    private void Update()
    {
        if (!matInner) return;

        if (activeColorData == null) return;
        
        matInner.color = Color.Lerp(matInner.color, activeColorData.innerColor, Time.deltaTime * transitionSpeed);
        matOuter.color = Color.Lerp(matOuter.color, activeColorData.outerColor, Time.deltaTime * transitionSpeed);;
    }

    private void OnObjectSpawned(GameObject fx)
    {
        var rend = fx.GetComponent<Renderer>();
        matInner = rend.materials[0];
        matOuter = rend.materials[1];

        matInner.color = Color.black;
        matOuter.color = Color.black;
    }

    public void RegisterColor(Component owner, int priority, Color innerColor, Color outerColor)
    {
        if (colorDatas.TryGetValue(owner, out var data))
        {
            data.priority = priority;
            data.innerColor = innerColor;
            data.outerColor = outerColor;
        }
        else
        {
            var colorData = new ColorData(priority, innerColor, outerColor);
            colorDatas.Add(owner, colorData);
        }
        
        foreach (var colorData in colorDatas)
        {
            if (activeColorData == null || activeColorData.priority < colorData.Value.priority)
            {
                activeColorData = colorData.Value;
            }
        }
    }

    public void RemoveColorData(Component owner)
    {
        colorDatas.Remove(owner);
    }

    private class ColorData
    {
        public int priority;
        public Color innerColor;
        public Color outerColor;

        public ColorData(int priority, Color innerColor, Color outerColor)
        {
            this.priority = priority;
            this.innerColor = innerColor;
            this.outerColor = outerColor;
        }
    }
}