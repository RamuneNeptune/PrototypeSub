using UnityEngine;

public class Test : MonoBehaviour
{
    public Renderer rend;
    public AnimationCurve noiseMulOverTime;
    public float duration;
    public bool invertDirection;

    private float currentTime;

    private void Update()
    {
        if (currentTime < duration)
        {
            currentTime += Time.deltaTime;
        }
        else if (currentTime > duration)
        {
            currentTime = 0;
        }

        float normaliedValue = currentTime / duration;
        if (invertDirection)
        {
            normaliedValue = 1 - normaliedValue;
        }

        rend.material.SetFloat("_UVTarget", normaliedValue);
        rend.material.SetFloat("_NoiseMultiplier", noiseMulOverTime.Evaluate(normaliedValue));
    }
}
