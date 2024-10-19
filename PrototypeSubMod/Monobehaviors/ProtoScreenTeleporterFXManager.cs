using UnityEngine;

namespace PrototypeSubMod.Monobehaviors;

internal class ProtoScreenTeleporterFXManager : MonoBehaviour
{
    private static Color teleportInnerCol;
    private static Color teleportMiddleCol;
    private static Color teleportOuterCol;

    private TeleportScreenFX teleportFX;

    private void Start()
    {
        teleportFX = Camera.main.GetComponent<TeleportScreenFX>();
        teleportInnerCol = teleportFX.mat.GetColor("_ColorCenter");
        teleportMiddleCol = teleportFX.mat.GetColor("_ColorStrength");
        teleportOuterCol = teleportFX.mat.GetColor("_ColorOuter");
    }

    public void SetColors(Color innerColor, Color middleColor, Color outerColor)
    {
        teleportFX.mat.SetColor("_ColorCenter", innerColor);
        teleportFX.mat.SetColor("_ColorStrength", middleColor);
        teleportFX.mat.SetColor("_ColorOuter", outerColor);
    }

    public void ResetColors()
    {
        teleportFX.mat.SetColor("_ColorCenter", teleportInnerCol);
        teleportFX.mat.SetColor("_ColorStrength", teleportMiddleCol);
        teleportFX.mat.SetColor("_ColorOuter", teleportOuterCol);
    }
}
