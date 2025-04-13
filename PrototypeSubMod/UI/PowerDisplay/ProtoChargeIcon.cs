using UnityEngine;
using UnityEngine.UI;

namespace PrototypeSubMod.UI.PowerDisplay;

public class ProtoChargeIcon : MonoBehaviour
{
    [SerializeField] private Image icon;

    public void SetIconAlpha(float alpha)
    {
        icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, alpha);
    }
}