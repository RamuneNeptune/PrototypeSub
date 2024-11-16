using SubLibrary.CyclopsReferencers;
using SubLibrary.Handlers;
using System.Collections;
using UnityEngine;

namespace PrototypeSubMod.Monobehaviors;

internal class CyclopsReferenceCaller : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return CyclopsReferenceHandler.EnsureCyclopsReference();

        foreach (var item in GetComponentsInChildren<ICyclopsReferencer>(true))
        {
            item.OnCyclopsReferenceFinished(CyclopsReferenceHandler.CyclopsReference);
        }
    }
}
