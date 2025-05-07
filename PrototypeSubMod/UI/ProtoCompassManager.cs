using SubLibrary.UI;
using UnityEngine;
using UnityEngine.UI;

namespace PrototypeSubMod.UI;

public class ProtoCompassManager : MonoBehaviour, IUIElement
{
    [SerializeField] private SubRoot subRoot;
    [SerializeField] private Image compassImage;
    [SerializeField] private Sprite[] cardinalSprites;
    
    public void UpdateUI()
    {
        int index = Mathf.RoundToInt((subRoot.transform.eulerAngles.y / 360) * 8);
        compassImage.sprite = cardinalSprites[index];
    }

    public void OnSubDestroyed()
    {
        
    }
}