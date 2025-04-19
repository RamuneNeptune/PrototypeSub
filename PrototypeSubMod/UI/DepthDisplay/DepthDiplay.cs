using SubLibrary.UI;
using UnityEngine;

namespace PrototypeSubMod.UI.DepthDisplay;

public class ProtoDepthDisplay : MonoBehaviour, IUIElement
{
    [SerializeField] private CrushDamage crushDamage;
    [SerializeField] private Transform[] leftBars;
    [SerializeField] private Transform[] rightBars;
    
    public void UpdateUI()
    {
        int segmentCount = Mathf.FloorToInt(crushDamage.GetDepth() / crushDamage.crushDepth * 10);
        for (int i = 0; i < leftBars.Length; i++)
        {
            bool active = (leftBars.Length - i) <= segmentCount;
            leftBars[i].gameObject.SetActive(active);
            rightBars[i].gameObject.SetActive(active);
        }
    }

    public void OnSubDestroyed() { }
}