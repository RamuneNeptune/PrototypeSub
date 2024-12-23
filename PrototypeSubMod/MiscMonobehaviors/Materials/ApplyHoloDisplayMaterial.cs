using SubLibrary.CyclopsReferencers;
using SubLibrary.Materials.Tags;
using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.Materials;

internal class ApplyHoloDisplayMaterial : MonoBehaviour, ICyclopsReferencer
{
    [SerializeField] private ApplyMode applyMode;
    [SerializeField] private Color holoColor = new Color(0.4559f, 0.9082f, 1f, 1f);

    [Header("Single Rend Mode")]
    [SerializeField] private Renderer singleRenderer;

    private void Awake()
    {
        foreach (var item in GetComponentsInChildren<SubExteriorTag>())
        {
            Destroy(item);
        }

        foreach (var item in GetComponentsInChildren<SubWindowTag>())
        {
            Destroy(item);
        }
    }

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
                foreach (var rend in GetComponentsInChildren<Renderer>(true))
                {
                    var mats = rend.materials;
                    for (int i = 0; i < mats.Length; i++)
                    {
                        mats[i] = newMaterial;
                    }
                    rend.materials = mats;
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
