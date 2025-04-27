using Nautilus.Utility;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace PrototypeSubMod.MiscMonobehaviors.Materials;

public class ApplySNMatsToGraphics : MonoBehaviour
{
    [FormerlySerializedAs("grahics")] [SerializeField] private Graphic[] graphics;
    [SerializeField] private float shininess;
    [SerializeField] private float specularIntensity;
    [SerializeField] private MaterialUtils.MaterialType materialType;
    
    private void Start()
    {
        Graphic[] graphicArray = graphics;
        if (graphics != null && graphics.Length > 0)
        {
            graphics = GetComponentsInChildren<Graphic>(true);
        }
        
        foreach (var graphic in graphicArray)
        {
            if (graphic.material == null) continue;
            
            MaterialUtils.ApplyUBERShader(graphic.material, shininess, specularIntensity, 0, materialType);
        }
    }
}