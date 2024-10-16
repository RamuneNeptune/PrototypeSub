using System.Collections;
using System.Linq;
using UnityEngine;

namespace PrototypeSubMod.Monobehaviors;

internal class LightColorHandler : MonoBehaviour
{
    [SerializeField] private LightingController controller;
    [SerializeField] private float fadeSpeed = 1f;

    private Color[] originalColors;
    private Color[] targetColors;

    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        originalColors = new Color[controller.lights.Length];

        for (int i = 0; i < controller.lights.Length; i++)
        {
            originalColors[i] = controller.lights[i].light.color;
        }

        targetColors = originalColors;
    }

    private void Update()
    {
        for (int i = 0; i < controller.lights.Length; i++)
        {
            var light = controller.lights[i].light;
            light.color = Color.Lerp(light.color, targetColors[i], Time.deltaTime * fadeSpeed);
        }
    }

    public void SetTempColor(Color color)
    {
        targetColors = Enumerable.Repeat(color, controller.lights.Length).ToArray();
    }

    public void ResetColor()
    {
        for (int i = 0; i < controller.lights.Length; i++)
        {
            targetColors[i] = originalColors[i];
        }
    }
}
