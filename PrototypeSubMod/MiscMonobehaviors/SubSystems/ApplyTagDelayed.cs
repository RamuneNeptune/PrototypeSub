using System.Collections;
using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.SubSystems;

public class ApplyTagDelayed : MonoBehaviour
{
    [SerializeField] private string tag;
    [SerializeField] private int frameDelay = 2;
    
    private IEnumerator Start()
    {
        for (int i = 0; i < frameDelay; i++)
        {
            yield return new WaitForEndOfFrame();
        }

        transform.tag = tag;
    }
}