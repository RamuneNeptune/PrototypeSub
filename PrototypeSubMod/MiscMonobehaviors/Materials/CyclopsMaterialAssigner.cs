using SubLibrary.Handlers;
using SubLibrary.Monobehaviors;
using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.Materials;

internal class CyclopsMaterialAssigner : PrefabModifier
{
    [SerializeField] private string objectPath;
    [SerializeField] private int materialIndex;
    [SerializeField] private MeshRenderer renderer;

    private void OnValidate()
    {
        if (!renderer) TryGetComponent(out renderer);
    }

    public override void OnLateMaterialOperation()
    {
        var meshRenderer = CyclopsReferenceHandler.CyclopsReference.transform.Find(objectPath).GetComponent<MeshRenderer>();
        renderer.material = meshRenderer.materials[materialIndex];
    }
}
