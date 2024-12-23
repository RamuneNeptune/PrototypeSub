using UnityEngine;

namespace PrototypeSubMod.StatsTerminal;

internal class ProtoStatsTerminal : MonoBehaviour
{
    [SerializeField] private Animator animator;
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

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.Equals(Player.main.gameObject)) return;

        animator.SetBool("TerminalOpen", true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.Equals(Player.main.gameObject)) return;

        animator.SetBool("TerminalOpen", false);
    }

    private void Update()
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
