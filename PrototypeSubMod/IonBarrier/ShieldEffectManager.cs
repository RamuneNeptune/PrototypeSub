using SubLibrary.CyclopsReferencers;
using UnityEngine;

namespace PrototypeSubMod.IonBarrier;

internal class ShieldEffectManager : MonoBehaviour, ICyclopsReferencer
{
    [SerializeField] private Renderer[] renderersToApply;
    [SerializeField] private float startIntensity;
    [SerializeField] private float enabledSize = -0.02f;
    [SerializeField] private Color mainColor;
    [SerializeField] private Color solidColor;
    [SerializeField] private Vector4 scrollSpeed;
    [SerializeField] private Vector4 wobbleParams;
    [SerializeField] private float tempColorTransitionSpeed = 1f;

    private Color tempMainCol;
    private Color tempSolidCol;
    private bool tempColActive;
    private float transitionTimeOut;
    private float currentTransitionTime;

    private void Start()
    {
        transitionTimeOut = (1 / tempColorTransitionSpeed) * 8f;
        currentTransitionTime = transitionTimeOut;
    }

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

    private void Update()
    {
        if (currentTransitionTime < transitionTimeOut)
        {
            currentTransitionTime += Time.deltaTime;
        }
        else
        {
            return;
        }

        Color targetMainCol = tempColActive ? tempMainCol : mainColor;
        Color targetSolidCol = tempColActive? tempSolidCol : solidColor;

        foreach (var rend in renderersToApply)
        {
            Color mainCol = Color.Lerp(rend.material.GetColor("_Color"), targetMainCol, tempColorTransitionSpeed * Time.deltaTime);
            Color solidCol = Color.Lerp(rend.material.GetColor("_Color"), targetMainCol, tempColorTransitionSpeed * Time.deltaTime);
            rend.material.SetColor("_Color", mainCol);
            rend.material.SetColor("_SolidColor", solidCol);
        }
    }

    public void SetTempColor(Color mainCol, Color solidColor)
    {
        tempMainCol = mainCol;
        tempSolidCol = solidColor;
        tempColActive = true;

        currentTransitionTime = 0;
    }

    public void ClearTempColor()
    {
        tempColActive = false;
    }
}
