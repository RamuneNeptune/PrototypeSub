﻿using System;
using UnityEngine;

namespace PrototypeSubMod.PrototypeStory;

public class EndingMusicManager : MonoBehaviour, IScheduledUpdateBehaviour
{
    [SerializeField] private FMOD_CustomEmitter leadUpMusic;
    [SerializeField] private FMOD_CustomEmitter climaxMusic;
    [SerializeField] private float climaxActivationDistance = 1000;

    private bool endingStarted;
    
    private void Start()
    {
        ProtoStoryLocker.onEndingStart += OnEndingStart;
    }

    private void OnEndingStart()
    {
        climaxMusic.Play();
        leadUpMusic.Stop();
        endingStarted = true;
    }

    public void OnFadeToCredits()
    {
        climaxMusic.Stop();
    }
    
    public void ScheduledUpdate()
    {
        if (endingStarted) return;
        
        bool inDistance = (Player.main.transform.position - Plugin.STORY_END_POS).sqrMagnitude < climaxActivationDistance * climaxActivationDistance;
        bool inVoid = Player.main.GetBiomeString() == "void";
        
        if (inVoid && inDistance && !leadUpMusic.playing)
        {
            leadUpMusic.Play();
        }
        else if (leadUpMusic.playing)
        {
            leadUpMusic.Stop();
        }
    }
    
    private void OnDestroy()
    {
        ProtoStoryLocker.onEndingStart -= OnEndingStart;
    }

    public virtual void OnEnable()
    {
        UpdateSchedulerUtils.Register(this);
    }

    public virtual void OnDisable()
    {
        UpdateSchedulerUtils.Deregister(this);
    }

    public string GetProfileTag() => "ProtoEndingMusicManager";

    public int scheduledUpdateIndex { get; set; }
}