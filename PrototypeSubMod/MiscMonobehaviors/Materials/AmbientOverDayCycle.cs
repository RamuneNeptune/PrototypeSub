using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.Materials;

internal class AmbientOverDayCycle : MonoBehaviour
{
    [SerializeField] private Gradient ambientOverDay;
    [SerializeField] private Renderer renderer;

    private void FixedUpdate()
    {
        Color color = ambientOverDay.Evaluate(DayNightCycle.main.GetDayScalar());
        renderer.material.SetColor("_AmbientColor", color);
    }
}
