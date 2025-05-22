using System;
using PrototypeSubMod.Utility;
using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.SubSystems;

public class ApplyTagTechType : MonoBehaviour
{
    [SerializeField] private TechTag techTag;
    [SerializeField] private DummyTechType techType;

    private void OnValidate()
    {
        if (!techTag) TryGetComponent(out techTag);
    }

    private void Start()
    {
        techTag.type = techType.TechType;
    }
}