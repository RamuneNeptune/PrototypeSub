using UnityEngine;

namespace PrototypeSubMod.StatsTerminal;

internal class ProtoHealthStat : MonoBehaviour, IStatistic
{
    [SerializeField] private LiveMixin liveMixin;
    [SerializeField] private GameObject modelsParent;
    [SerializeField] private Gradient holoColorOverHealth;
    [SerializeField] private float colorTransitionSpeed;

    private Renderer[] renderers;
    private Color currentColor;

    private void Start()
    {
        renderers = modelsParent.GetComponentsInChildren<Renderer>();
    }

    public void UpdateStat()
    {
        currentColor = renderers[0].material.color;
        var targetColor = Color.Lerp(currentColor, holoColorOverHealth.Evaluate(liveMixin.health / liveMixin.maxHealth), Time.deltaTime * colorTransitionSpeed);

        foreach (var rend in renderers)
        {
            var mats = rend.materials;
            for (int i = 0; i < mats.Length; i++)
            {
                mats[i].color = targetColor;
            }
            rend.materials = mats;
        }
    }
}
