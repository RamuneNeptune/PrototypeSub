using UnityEngine;

namespace PrototypeSubMod.StatsTerminal;

internal class ProtoWarningPing : MonoBehaviour
{
    public CyclopsHolographicHUD_WarningPings.WarningTypes warningType;
    public GameObject damageText;
    public GameObject labelDot;
    public GameObject warningPing;
    public LineRenderer lineRenderer;
    public Animator animator;

    private BehaviourLOD lod;
    private Transform parent;
    private bool despawning;

    private void Start()
    {
        if (warningType != CyclopsHolographicHUD_WarningPings.WarningTypes.Damage) return;
        if (damageText == null) return;
        if (labelDot == null) return;
        if (lineRenderer == null) return;

        var textRect = damageText.transform.GetChild(0).GetComponent<RectTransform>();
        textRect.localScale = Vector3.one * 23.33f;
        float xPos = -1167.760f * textRect.localPosition.x;
        textRect.localPosition = new Vector3(xPos, 5.870f, -189.170f);

        var labelRect = warningPing.GetComponent<RectTransform>();
        labelRect.sizeDelta = new Vector2(1600, 1584);

        Vector3 parentOffset = (parent.InverseTransformPoint(transform.localPosition) - parent.position).normalized;
        Vector3 dotPos = parent.TransformPoint(transform.position + parentOffset * 0.35f);
        labelDot.transform.localPosition = dotPos;

        var color = new Color(0.835f, 1f, 0.792f);
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        lineRenderer.material.color = color;

        lineRenderer.SetPositions(new[] { transform.position, labelDot.transform.position });
        Vector3 textOffset = new Vector3(0, 0, 58f);

        if (labelDot.transform.localPosition.z < transform.localPosition.z)
        {
            textOffset *= -1;
        }

        damageText.transform.localPosition = labelDot.transform.localPosition + textOffset;
    }

    private void Update()
    {
        if (lod == null || !lod.IsFull()) return;

        warningPing.transform.LookAt(MainCamera.camera.transform.position);
        if (damageText != null && this.warningType == CyclopsHolographicHUD_WarningPings.WarningTypes.Damage)
        {
            labelDot.transform.LookAt(MainCamera.camera.transform.position);
            labelDot.transform.Rotate(new Vector3(0f, 180f, 0f), Space.Self);
            damageText.transform.LookAt(MainCamera.camera.transform.position);
            damageText.transform.Rotate(new Vector3(0f, 180f, 0f), Space.Self);

            lineRenderer.SetPositions(new[] { transform.position, labelDot.transform.position });
        }
    }

    public void SetLOD(BehaviourLOD lod)
    {
        this.lod = lod;
    }

    public void SetParent(Transform parent)
    {
        this.parent = parent;
    }

    private void Despawn()
    {
        Destroy(gameObject);
    }

    public void DespawnIcon()
    {
        if (despawning) return;
        despawning = true;
        animator.SetTrigger("Death");
        Invoke(nameof(Despawn), 3f);
    }
}
