using System.Collections;
using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.SubSystems;

public class ApplyTagDelayed : MonoBehaviour
{
    [SerializeField] private string tag;
    [SerializeField] private int frameDelay = 2;
    [SerializeField] private bool allChildren;
    
    private IEnumerator Start()
    {
        for (int i = 0; i < frameDelay; i++)
        {
            yield return new WaitForEndOfFrame();
        }

        transform.tag = tag;
        
        if (!allChildren) yield break;

        ApplyTagsRecursive(transform);
    }

    private void ApplyTagsRecursive(Transform root)
    {
        foreach (Transform child in root)
        {
            child.tag = tag;
            if (child.childCount > 0) ApplyTagsRecursive(child);
        }
    }
}