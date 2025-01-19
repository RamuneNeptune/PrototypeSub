using Nautilus.Utility;
using PrototypeSubMod.Utility;
using System.Collections;
using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.Materials;

internal class ApplySNProtoShaders : MonoBehaviour
{
    [SerializeField] private GameObject applyTo;
    [SerializeField] private float shininess = 4;
    [SerializeField] private float specularIntensity = 3;
    [SerializeField] private float glowStrength = 1;

    private void OnValidate()
    {
        if (applyTo == null) applyTo = gameObject;
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => MaterialUtils.IsReady);

        MaterialUtils.ApplySNShaders(applyTo, shininess, specularIntensity, glowStrength, new ProtoMaterialModifier(specularIntensity, 0));
    }
}
