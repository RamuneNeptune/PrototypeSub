using System;
using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.Materials;

public interface IMaterialModifier
{
    public event Action<GameObject> onEditMaterial;
}