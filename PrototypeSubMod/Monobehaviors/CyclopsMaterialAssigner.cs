using SubLibrary.CyclopsReferencers;
using UnityEngine;

namespace PrototypeSubMod.Monobehaviors;

internal class CyclopsMaterialAssigner : MonoBehaviour, ICyclopsReferencer
{
    [SerializeField] private string objectPath;
    [SerializeField] private int materialIndex;
    [SerializeField] private MeshRenderer renderer;

    private void OnValidate()
    {
        if (!renderer) TryGetComponent(out renderer);
    }

    public void OnCyclopsReferenceFinished(GameObject cyclops)
    {
        var meshRenderer = cyclops.transform.Find(objectPath).GetComponent<MeshRenderer>();

        renderer.material = meshRenderer.materials[materialIndex];
    }
}
