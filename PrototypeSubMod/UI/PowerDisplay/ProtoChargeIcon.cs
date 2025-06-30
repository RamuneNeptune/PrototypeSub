using UnityEngine;
using UnityEngine.UI;

namespace PrototypeSubMod.UI.PowerDisplay;

public class ProtoChargeIcon : MonoBehaviour
{
    [SerializeField] private Image icon;

    public void SetColor(Color color)
    {
        icon.color = color;
    }

    public void SetSprite(Sprite sprite)
    {
        icon.sprite = sprite;
    }
}