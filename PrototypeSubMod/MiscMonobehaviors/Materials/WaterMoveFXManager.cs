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
    private Renderer rend;
    
    private void Start()
    {
        prefabSpawner.onEditMaterial += OnObjectSpawned;
    }

    private void Update()
    {
        if (!rend) return;

        if (activeColorData == null) return;
        
        var materials = rend.materials;
        
        materials[0].color = Color.Lerp(materials[0].color, activeColorData.innerColor, Time.deltaTime * transitionSpeed);
        materials[1].color = Color.Lerp(materials[1].color, activeColorData.outerColor, Time.deltaTime * transitionSpeed);

        rend.materials = materials;
    }

    private void OnObjectSpawned(GameObject fx)
    {
        rend = fx.GetComponent<Renderer>();
        
        var materials = rend.materials;

        materials[0].color = Color.black;
        materials[1].color = Color.black;
        
        rend.materials = materials;
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