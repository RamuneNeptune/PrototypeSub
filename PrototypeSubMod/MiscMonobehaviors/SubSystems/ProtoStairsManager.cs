using System;
using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.SubSystems;

public class ProtoStairsManager : MonoBehaviour
{
    private static readonly int StairsActive = Animator.StringToHash("StairsActive");

    [SerializeField] private SubRoot subRoot;
    [SerializeField] private Animator stairsAnimator;
    [SerializeField] private FMOD_CustomEmitter stairsUpEmitter;
    [SerializeField] private FMOD_CustomEmitter stairsDownEmitter;

    private bool queuedNoPlaySfx;
    private bool stairsActive;
    
    private void Start()
    {
        Player.main.playerRespawnEvent.AddHandler(gameObject, OnRespawn);
    }

    private void OnEnable()
    {
        queuedNoPlaySfx = true;
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
        queuedNoPlaySfx = true;
        SetStairsActive(true);
    }

    public void PlayStairsUpSfx()
    {
        if (!queuedNoPlaySfx) stairsUpEmitter.Play();
        queuedNoPlaySfx = false;
    }
    
    public void PlayStairsDownSfx()
    {
        if (!queuedNoPlaySfx) stairsDownEmitter.Play();
        queuedNoPlaySfx = false;
    }
}