using UnityEngine;
using UnityEngine.UI;

namespace PrototypeSubMod.Credits;

public class ProtoScreenFadeManager : MonoBehaviour
{
    [SerializeField] private Image fadeImage;

    private bool fading;
    private float currentFadeTime;
    private float targetFadeTime;
    private Color targetColor;

    private void Update()
    {
        if (!fading) return;

        if (currentFadeTime <   targetFadeTime)
        {
            currentFadeTime += Time.deltaTime;
        }
        else
        {
            fading = false;
        }
        
        fadeImage.color = Color.Lerp(fadeImage.color, targetColor, currentFadeTime / targetFadeTime);
    }

    public void FadeIn(float duration)
    {
        fading = true;
        currentFadeTime = duration;
        targetFadeTime = duration;
        targetColor = Color.black;
    }
    
    public void FadeOut(float duration)
    {
        fading = true;
        currentFadeTime = duration;
        targetFadeTime = duration;
        targetColor = new Color(0, 0, 0, 0);
    }
}