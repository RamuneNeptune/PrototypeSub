using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.Materials;

internal class LightMultiplierOverDay : MonoBehaviour
{
    [SerializeField] private AnimationCurve multiplierOverDay;
    [SerializeField] private Renderer[] renderers;

    private void FixedUpdate()
    {
        float multiplier = multiplierOverDay.Evaluate(DayNightCycle.main.GetDayScalar());
        foreach (var rend in renderers)
        {
            rend.material.SetFloat("_NightMultiplier", multiplier);
        }
    }
}
