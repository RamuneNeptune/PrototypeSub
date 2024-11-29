using SubLibrary.CyclopsReferencers;
using System;
using UnityEngine;

namespace PrototypeSubMod.Monobehaviors;

internal class ApplyCyclopsGlass : MonoBehaviour, ICyclopsReferencer
{
    [SerializeField] private Renderer renderer;

    private void OnValidate()
    {
        if (!renderer) TryGetComponent(out renderer);
    }

    public void OnCyclopsReferenceFinished(GameObject cyclops)
    {
        var glassObj = cyclops.transform.Find("CyclopsMeshStatic/undamaged/cyclops_LOD0/Cyclops_submarine_exterior_glass");
        var rend = glassObj.GetComponent<Renderer>();
        renderer.material = new(rend.material);
    }
}
