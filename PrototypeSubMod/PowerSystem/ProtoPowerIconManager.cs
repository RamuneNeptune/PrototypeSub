using System;
using System.Collections.Generic;
using UnityEngine;

namespace PrototypeSubMod.PowerSystem;

public class ProtoPowerIconManager : MonoBehaviour
{
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private List<IconData> iconDatas;

    [HideInInspector, SerializeField] public string[] techTypeNames;
    [HideInInspector, SerializeField] public Sprite[] sprites;

    private Dictionary<TechType, Sprite> techTypeIcons = new();
    
    private void OnValidate()
    {
        techTypeNames = new string[iconDatas.Count];
        sprites = new Sprite[iconDatas.Count];
        for (int i = 0; i < iconDatas.Count; i++)
        {
            var icon = iconDatas[i];
            techTypeNames[i] = icon.techTypeName;
            sprites[i] = icon.sprite;
        }
    }

    private void Start()
    {
        for (int i = 0; i < techTypeNames.Length; i++)
        {
            var techTypeName = techTypeNames[i];
            var techType = (TechType)Enum.Parse(typeof(TechType), techTypeName);
            techTypeIcons.Add(techType, sprites[i]);
        }
    }

    public Sprite GetSpriteForTechType(TechType techType)
    {
        if (techTypeIcons.TryGetValue(techType, out var sprite))
        {
            return sprite;
        }
        
        return defaultSprite;
    }
}

[Serializable]
public struct IconData
{
    [HideInInspector] public TechType techType;
    public string techTypeName;
    public Sprite sprite;

    public IconData(string techTypeName, Sprite sprite)
    {
        this.techTypeName = techTypeName;
        this.sprite = sprite;
    }
}