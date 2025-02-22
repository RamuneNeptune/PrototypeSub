using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.Materials
{
    internal class FogDistOverDay : MonoBehaviour
    {
        [SerializeField] private AnimationCurve distOverDay;
        [SerializeField] private Renderer[] renderers;

        private void FixedUpdate()
        {
            float dist = distOverDay.Evaluate(DayNightCycle.main.GetDayScalar());
            foreach (var rend in renderers)
            {
                rend.material.SetFloat("_FogMaxDist", dist);
            }
        }
    }
}
