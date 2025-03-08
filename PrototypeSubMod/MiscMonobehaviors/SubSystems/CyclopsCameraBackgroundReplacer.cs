using UnityEngine;
using UnityEngine.UI;

namespace PrototypeSubMod.MiscMonobehaviors.SubSystems;

internal class CyclopsCameraBackgroundReplacer : MonoBehaviour
{
    private const string CAMERA_CYCLOPS_PATH = "Content/CameraCyclops";
    private const string TITLE_UNDERLINE_PATH = "Title/TitleUnderline";
    private const string FRAME_LEFT_PATH = "FrameLeft";
    private const string FRAME_RIGHT_PATH = "FrameRight";
    private const string CROSSHAIR_PATH = "Crosshair";

    [SerializeField] private string imageHolderName;
    [SerializeField] private Sprite backgroundSprite;
    [SerializeField] private Vector2 spriteScale = Vector2.one;

    private Image backgroundImage;
    private GameObject titleUnderline;
    private GameObject frameLeft;
    private GameObject frameRight;
    private GameObject crosshair;

    private void Start()
    {
        Transform root = uGUI_CameraCyclops.main.transform.Find(CAMERA_CYCLOPS_PATH);
        titleUnderline = root.transform.Find(TITLE_UNDERLINE_PATH).gameObject;
        frameLeft = root.transform.Find(FRAME_LEFT_PATH).gameObject;
        frameRight = root.transform.Find(FRAME_RIGHT_PATH).gameObject;
        crosshair = root.transform.Find(CROSSHAIR_PATH).gameObject;

        var imageHolder = root.transform.Find(imageHolderName);
        if (imageHolder == null)
        {
            var holder = new GameObject(imageHolderName);
            imageHolder = holder.AddComponent<RectTransform>();
            imageHolder.SetParent(root);
            imageHolder.localPosition = Vector3.zero;
            imageHolder.localScale = new Vector3(spriteScale.x, spriteScale.y, 1);
            imageHolder.localRotation = Quaternion.identity;
            imageHolder.gameObject.AddComponent<Image>();
        }

        backgroundImage = imageHolder.GetComponent<Image>();
    }

    public void OnCamerasEntered()
    {
        backgroundImage.gameObject.SetActive(true);
        backgroundImage.sprite = backgroundSprite;

        titleUnderline.SetActive(false);
        frameLeft.SetActive(false);
        frameRight.SetActive(false);
        crosshair.SetActive(false);
    }

    public void OnCamerasExited()
    {
        backgroundImage.gameObject.SetActive(false);

        titleUnderline.SetActive(true);
        frameLeft.SetActive(true);
        frameRight.SetActive(true);
        crosshair.SetActive(true);
    }
}
