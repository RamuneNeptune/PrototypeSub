using SubLibrary.UI;
using UnityEngine;
using UnityEngine.UI;

namespace PrototypeSubMod.UI.DepthDisplay;

public class ProtoDepthDisplay : MonoBehaviour, IUIElement
{
    [SerializeField] private CrushDamage crushDamage;
    [SerializeField] private Image leftMask;
    [SerializeField] private Image rightMask;
    [SerializeField] private float[] fillIncrements;

    private int segmentCountLastFrame = -1;
    
    public void UpdateUI()
    {
        int segmentCount = Mathf.FloorToInt(crushDamage.GetDepth() / crushDamage.crushDepth * 10);
        segmentCount = Mathf.Clamp(segmentCount, 0, fillIncrements.Length - 1);
        
        if (segmentCount != segmentCountLastFrame)
        {
            leftMask.fillAmount = fillIncrements[segmentCount];
            rightMask.fillAmount = fillIncrements[segmentCount];
        }

        segmentCountLastFrame = segmentCount;
    }

    public void OnSubDestroyed() { }
}