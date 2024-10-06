using UnityEngine;
using UnityEngine.UI;

namespace PrototypeSubMod.Monobehaviors;

internal class CyclopsCameraIconReplacer : MonoBehaviour
{
    private const string BG_PATH = "Content/CameraCyclops/DirectionIndicator/DirectionCyclops";

    [SerializeField] private Sprite replacementSprite;

    private Sprite originalSprite;
    private Image backgroundImage;

    private void Start()
    {
        backgroundImage = uGUI_CameraCyclops.main.transform.Find(BG_PATH).GetComponent<Image>();
        originalSprite = backgroundImage.sprite;
    }

    public void OnCamerasEntered()
    {
        backgroundImage.sprite = replacementSprite;
    }

    public void OnCamerasExited()
    {
        backgroundImage.sprite = originalSprite;
    }
}
