using UnityEngine;
using uSky;

namespace PrototypeSubMod.MiscMonobehaviors.Materials;

internal class AmbientOverDayCycle : MonoBehaviour
{
    [SerializeField] private AnimationCurve ambientMultiplierOverDay;
    [SerializeField] private Renderer[] renderers;

    private uSkyLight skyLight;

    private void Start()
    {
        skyLight = uSkyManager.main.GetComponent<uSkyLight>();
    }

    private void FixedUpdate()
    {
        float inverseEclipse = 1 - skyLight.uSM.Eclipse();
        float ambientMultiplier = ambientMultiplierOverDay.Evaluate(skyLight.currentTime) * skyLight.ambientLight;
        RenderSettings.ambientSkyColor = skyLight.CurrentSkyColor * inverseEclipse * ambientMultiplier;
        RenderSettings.ambientEquatorColor = skyLight.CurrentEquatorColor * inverseEclipse * ambientMultiplier;
        RenderSettings.ambientGroundColor = skyLight.CurrentGroundColor * inverseEclipse * ambientMultiplier;
    }
}
