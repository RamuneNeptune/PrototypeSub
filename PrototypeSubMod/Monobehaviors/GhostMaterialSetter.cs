using UnityEngine;

namespace PrototypeSubMod.Monobehaviors;

internal class GhostMaterialSetter : MonoBehaviour
{
    [SerializeField] private VFXConstructing vfxConstructing;
    [SerializeField] private Color ghostMatColor;

    public void OnConstructionStarted()
    {
        vfxConstructing.ghostMaterial = new Material(vfxConstructing.ghostMaterial);
        vfxConstructing.ghostMaterial.color = ghostMatColor;

        vfxConstructing.ghostOverlay.ApplyOverlay(vfxConstructing.ghostMaterial, "VFXConstructing", false);
    }
}
