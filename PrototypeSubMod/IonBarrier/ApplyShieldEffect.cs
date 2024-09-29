using SubLibrary.CyclopsReferencers;
using UnityEngine;

namespace PrototypeSubMod.IonBarrier;

internal class ApplyShieldEffect : MonoBehaviour, ICyclopsReferencer
{
    [SerializeField] private Renderer[] renderersToApply;
    [SerializeField] private float startIntensity;
    [SerializeField] private float enabledSize = -0.02f;
    [SerializeField] private Color mainColor;
    [SerializeField] private Color solidColor;
    [SerializeField] private Vector4 scrollSpeed;
    [SerializeField] private Vector4 wobbleParams;

    public void OnCyclopsReferenceFinished(GameObject cyclops)
    {
        string pathToShieldObj = "FX/x_Cyclops_GlassShield";
        Transform shieldObj = cyclops.transform.Find(pathToShieldObj);

        Renderer renderer = shieldObj.GetComponent<Renderer>();
        Material newMaterial = new Material(renderer.material);
        newMaterial.SetColor("_Color", mainColor);
        newMaterial.SetColor("_SolidColor", solidColor);
        newMaterial.SetFloat("_Intensity", startIntensity);
        newMaterial.SetVector("_ScrollSpeed", scrollSpeed);
        newMaterial.SetVector("_WobbleParams", wobbleParams);
        newMaterial.SetFloat("_EnabledSize", enabledSize);

        foreach (var rend in renderersToApply)
        {
            rend.material = newMaterial;
            rend.GetComponent<MeshFilter>().mesh = shieldObj.GetComponent<MeshFilter>().mesh;
        }
    }
}
