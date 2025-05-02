using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.SubSystems;

public class ProtoStairsManager : MonoBehaviour
{
    private static readonly int StairsActive = Animator.StringToHash("StairsActive");
    
    [SerializeField] private Animator stairsAnimator;
    
    private bool stairsActive;
    
    private void Start()
    {
        Player.main.playerRespawnEvent.AddHandler(gameObject, OnRespawn);
    }

    private void OnRespawn(Player player)
    {
        SetStairsActive(false);
    }

    public void SetStairsActive(bool active)
    {
        stairsAnimator.SetBool(StairsActive, active);
        stairsActive = active;
    }

    public void ToggleStairsActive()
    {
        SetStairsActive(!stairsActive);
    }
}