using System;
using SubLibrary.CyclopsReferencers;
using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.Materials;

internal class ApplyCyclopsMaterial : MonoBehaviour, ICyclopsReferencer, IMaterialModifier
{
    public event Action<GameObject> onEditMaterial;
    
    [SerializeField] private Renderer rend;
    [SerializeField] private string pathToTarget;
    [SerializeField] private int copyIndex;
    [SerializeField] private int applyIndex;

    private Material[] mats;

    public void OnCyclopsReferenceFinished(GameObject cyclops)
    {
        var copyRend = cyclops.transform.Find(pathToTarget)?.GetComponent<Renderer>();
        if (copyRend == null)
        {
            throw new System.Exception($"Renderer/Object not found at {pathToTarget}");
        }

        mats = rend.materials;
        mats[applyIndex] = new Material(copyRend.materials[copyIndex]);
        rend.materials = mats;

        onEditMaterial?.Invoke(rend.gameObject);
    }

    private void OnDestroy()
    {
        foreach (var mat in mats)
        {
            Destroy(mat);
        }
    }
}
