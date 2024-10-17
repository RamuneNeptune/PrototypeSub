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
    private bool usingTempColors;

    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        originalColors = new Color[controller.lights.Length];

        for (int i = 0; i < controller.lights.Length; i++)
        {
            originalColors[i] = controller.lights[i].light.color;
        }
    }

    private void Update()
    {
        if (originalColors == null || targetColors == null) return;

        for (int i = 0; i < controller.lights.Length; i++)
        {
            var light = controller.lights[i].light;

            Color col = usingTempColors ? targetColors[i] : originalColors[i];
            light.color = Color.Lerp(light.color, col, Time.deltaTime * fadeSpeed);
        }
    }

    public void SetTempColor(Color color)
    {
        targetColors = Enumerable.Repeat(color, controller.lights.Length).ToArray();
        usingTempColors = true;
    }

    public void ResetColor()
    {
        usingTempColors = false;
    }
}
