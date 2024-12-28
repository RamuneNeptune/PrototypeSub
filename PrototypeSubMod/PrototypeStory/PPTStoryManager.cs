using Nautilus.Handlers;
using System;
using UnityEngine;

namespace PrototypeSubMod.PrototypeStory;

internal class PPTStoryManager : MonoBehaviour
{
    private static event Action OnFirstPlayerInteraction;

    public static void RegisterGoals()
    {
        StoryGoalHandler.RegisterCustomEvent("PlayerFirstPPTInteraction", () =>
        {
            OnFirstPlayerInteraction?.Invoke();
        });
    }

    private void OnEnable()
    {
        OnFirstPlayerInteraction += OnPlayerFirstIneract;
    }

    private void OnDisable()
    {
        OnFirstPlayerInteraction -= OnPlayerFirstIneract;
    }

    private void OnPlayerFirstIneract()
    {
        ErrorMessage.AddError($"Wow, it's their first interaction!");
    }
}
