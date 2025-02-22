using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.Materials;

internal class AmbientOverDayCycle : MonoBehaviour
{
    [SerializeField] private Gradient ambientOverDay;
    [SerializeField] private Renderer[] renderers;

    private void FixedUpdate()
    {
        Color color = ambientOverDay.Evaluate(DayNightCycle.main.GetDayScalar());
        foreach (var rend in renderers)
        {
            rend.material.SetColor("_AmbientColor", color);
        }
    }
}
