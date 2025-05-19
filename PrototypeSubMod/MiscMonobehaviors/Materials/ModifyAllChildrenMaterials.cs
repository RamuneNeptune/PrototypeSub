using System;
using System.Collections;
using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.Materials;

public class ModifyAllChildrenMaterials : MonoBehaviour, IMaterialModifier
{
    public event Action<GameObject> onEditMaterial;

    [SerializeField] private int frameDelay = 2;
    [SerializeField] private Behaviour[] enableAfterApplication;

    private void Awake()
    {
        if (frameDelay == -1)
        {
            UWE.CoroutineHost.StartCoroutine(ApplyDelayed());
        }
    }

    private void Start()
    {
        if (frameDelay >= 0)
        {
            UWE.CoroutineHost.StartCoroutine(ApplyDelayed());
        }
    }

    private IEnumerator ApplyDelayed()
    {
        for (int i = 0; i < frameDelay; i++)
        {
            yield return new WaitForEndOfFrame();
        }

        Plugin.Logger.LogInfo("Applying material edits!");
        foreach (var rend in GetComponentsInChildren<Renderer>(true))
        {
            onEditMaterial?.Invoke(rend.gameObject);
        }

        foreach (var behavior in enableAfterApplication)
        {
            behavior.enabled = true;
        }
    }
}