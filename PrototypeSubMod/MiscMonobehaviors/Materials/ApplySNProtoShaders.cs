using Nautilus.Utility;
using PrototypeSubMod.Utility;
using System.Collections;
using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.Materials;

internal class ApplySNProtoShaders : MonoBehaviour
{
    [SerializeField] private GameObject applyTo;

    private void OnValidate()
    {
        if (applyTo == null) applyTo = gameObject;
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => MaterialUtils.IsReady);

        MaterialUtils.ApplySNShaders(applyTo, modifiers: new ProtoMaterialModifier(3f, 0));
    }
}
