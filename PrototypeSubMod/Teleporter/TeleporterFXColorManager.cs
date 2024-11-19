using System.Collections;
using UnityEngine;

namespace PrototypeSubMod.Teleporter;

internal class TeleporterFXColorManager : MonoBehaviour
{
    [SerializeField] private Transform fxParent;

    private Renderer rend;
    private Color originalCol;

    private Color tempColor;
    private bool usingTempColor;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => fxParent.childCount > 0);
        yield return new WaitForEndOfFrame();

        rend = fxParent.GetComponentInChildren<Renderer>(true);
        originalCol = rend.material.GetColor("_ColorOuter");
    }

    private void Update()
    {
        if (rend == null) return;

        Color col = Color.Lerp(rend.material.GetColor("_ColorOuter"), usingTempColor ? tempColor : originalCol, Time.deltaTime);
        rend.material.SetColor("_ColorOuter", col);
    }

    public void SetTempColor(Color color)
    {
        usingTempColor = true;
        tempColor = color;
    }

    public void ResetColor()
    {
        usingTempColor = false;
    }
}
