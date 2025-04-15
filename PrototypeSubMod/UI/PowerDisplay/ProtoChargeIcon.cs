using UnityEngine;
using UnityEngine.UI;

namespace PrototypeSubMod.UI.PowerDisplay;

public class ProtoChargeIcon : MonoBehaviour
{
    [SerializeField] private Image icon;

    public void SetSprite(Sprite sprite)
    {
        icon.sprite = sprite;
    }
}