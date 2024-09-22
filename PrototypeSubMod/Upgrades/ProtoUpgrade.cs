using PrototypeSubMod.Utility;
using UnityEngine;

namespace PrototypeSubMod.Upgrades;

internal abstract class ProtoUpgrade : MonoBehaviour
{
    public DummyTechType techType;
    public bool unlockedAtStart;

    private void Awake()
    {
        if (!unlockedAtStart) return;

        KnownTech.Add(techType.TechType);
    }
}
