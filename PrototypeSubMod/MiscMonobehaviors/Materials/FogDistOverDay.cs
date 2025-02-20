using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.Materials
{
    internal class FogDistOverDay : MonoBehaviour
    {
        [SerializeField] private AnimationCurve distOverDay;
        [SerializeField] private Renderer renderer;

        private void FixedUpdate()
        {
            float dist = distOverDay.Evaluate(DayNightCycle.main.GetDayScalar());
            renderer.material.SetFloat("_FogMaxDist", dist);
        }
    }
}
