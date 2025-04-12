using UnityEngine;
using UnityEngine.UI;

namespace PrototypeSubMod.PowerSystem;

public class ProtoPowerRelay : MonoBehaviour
{
    private static readonly int PylonActive = Animator.StringToHash("PylonActive");
    
    [SerializeField] private Animator animator;
    [SerializeField] private ProtoPowerIconManager iconManager;
    [SerializeField] private GameObject iconCanvas;
    [SerializeField] private Image icon;
    
    public void SetRelayActive(bool active)
    {
        animator.SetBool(PylonActive, active);
        iconCanvas.SetActive(active);
    }

    public void SetSourceType(TechType techType)
    {
        icon.sprite = iconManager.GetSpriteForTechType(techType);
    }
}