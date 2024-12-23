using SubLibrary.CyclopsReferencers;
using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.Materials;

internal class ApplyHoloDisplayMaterial : MonoBehaviour, ICyclopsReferencer
{
    [SerializeField] private ApplyMode applyMode;
    [SerializeField] private Color holoColor = new Color(0.4559f, 0.9082f, 1f, 1f);

    [Header("Single Rend Mode")]
    [SerializeField] private Renderer singleRenderer;

    public void OnCyclopsReferenceFinished(GameObject cyclops)
    {
        var borderObj = cyclops.transform.Find("HolographicDisplay/HolographicDisplayVisuals/CyclopsMini_Mid/border");
        var copyRend = borderObj.GetComponent<Renderer>();

        var newMaterial = new Material(copyRend.material);
        newMaterial.color = holoColor;

        switch (applyMode)
        {
            case ApplyMode.SingleRend:
                singleRenderer.material = newMaterial;
                break;
            case ApplyMode.AllChildren:
                foreach (var rend in GetComponentsInChildren<Renderer>())
                {
                    rend.material = newMaterial;
                }
                break;
        }
    }

    private enum ApplyMode
    {
        SingleRend,
        AllChildren
    }
}
