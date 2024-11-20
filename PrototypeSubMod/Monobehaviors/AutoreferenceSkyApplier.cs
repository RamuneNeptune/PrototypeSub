using UnityEngine;

namespace PrototypeSubMod.Monobehaviors;

internal class AutoreferenceSkyApplier : SkyApplier
{
    new private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
    }
}
