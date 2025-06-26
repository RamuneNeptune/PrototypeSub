using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.SubSystems;

public class ProtoStairsManager : MonoBehaviour
{
    private static readonly int StairsActive = Animator.StringToHash("StairsActive");

    [SerializeField] private SubRoot subRoot;
    [SerializeField] private Animator stairsAnimator;
    [SerializeField] private FMOD_CustomEmitter stairsUpEmitter;
    [SerializeField] private FMOD_CustomEmitter stairsDownEmitter;

    private bool queuedNoPlaySFX;
    private bool stairsActive;
    
    private void Start()
    {
        Player.main.playerRespawnEvent.AddHandler(gameObject, OnRespawn);
        SetStairsActive(Player.main.currentSub != subRoot);
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

    // Called by SubRoot.OnPlayerEntered
    public void PlayerEnteredSub()
    {
        SetStairsActive(false);
    }

    // Called by SubRoot.OnPlayerExited
    public void SaveEngineStateAndPowerDown()
    {
        queuedNoPlaySFX = true;
        SetStairsActive(true);
    }

    public void PlayStairsUpSfx()
    {
        if (!queuedNoPlaySFX) stairsUpEmitter.Play();
        queuedNoPlaySFX = false;
    }
    
    public void PlayStairsDownSfx()
    {
        if (!queuedNoPlaySFX) stairsDownEmitter.Play();
        queuedNoPlaySFX = false;
    }
}