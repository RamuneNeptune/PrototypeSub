using System;
using PrototypeSubMod.PrototypeStory;
using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.SubSystems;

public class DestroyOnStoryEnd : MonoBehaviour
{
    private void Start()
    {
        ProtoStoryLocker.onEndingStart += OnStoryEnds;
    }

    private void OnStoryEnds()
    {
        Destroy(gameObject);
    }
    
    private void OnDestroy()
    {
        ProtoStoryLocker.onEndingStart -= OnStoryEnds;
    }
}