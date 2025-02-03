using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.Materials;

internal class LightMultiplierOverDay : MonoBehaviour
{
    [SerializeField] private AnimationCurve multiplierOverDay;
    [SerializeField] private Renderer renderer;

    private void FixedUpdate()
    {
        float multiplier = multiplierOverDay.Evaluate(DayNightCycle.main.GetDayScalar());
        renderer.material.SetFloat("_NightMultiplier", multiplier);
    }
}
