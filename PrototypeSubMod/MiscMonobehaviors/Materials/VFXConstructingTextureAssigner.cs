using SubLibrary.CyclopsReferencers;
using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.Materials;

public class VFXConstructingTextureAssigner : MonoBehaviour, ICyclopsReferencer
{
    public void OnCyclopsReferenceFinished(GameObject cyclops)
    {
        var vfxConstruct = GetComponent<VFXConstructing>();
        var cyclopsConstruct = cyclops.GetComponent<VFXConstructing>();
        
        vfxConstruct.alphaTexture = cyclopsConstruct.alphaTexture;
        vfxConstruct.alphaDetailTexture = cyclopsConstruct.alphaDetailTexture;
    }
}