using SubLibrary.CyclopsReferencers;
using SubLibrary.Handlers;
using System.Collections;
using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors;

internal class CyclopsReferenceCaller : MonoBehaviour
{
    private void Start()
    {
        UWE.CoroutineHost.StartCoroutine(CallReferences());
    }

    private IEnumerator CallReferences()
    {
        yield return CyclopsReferenceHandler.EnsureCyclopsReference();
        yield return new WaitForEndOfFrame();
        
        foreach (var item in GetComponentsInChildren<ICyclopsReferencer>(true))
        {
            item.OnCyclopsReferenceFinished(CyclopsReferenceHandler.CyclopsReference);
        }
    }
}
