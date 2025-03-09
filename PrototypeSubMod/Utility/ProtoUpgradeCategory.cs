using System.Collections.Generic;
using UnityEngine;

namespace PrototypeSubMod.Utility;

[CreateAssetMenu(fileName = "UpgradeCategory", menuName = "Prototype Sub/Upgrade Category")]
internal class ProtoUpgradeCategory : ScriptableObject
{
    public string localizationKey;
    public DummyTechType[] ownedTechTypes;

    public TechType[] GetTechTypes()
    {
        TechType[] types = new TechType[ownedTechTypes.Length];
        for (int i = 0; i < ownedTechTypes.Length; i++)
        {
            types[i] = ownedTechTypes[i].TechType;
        }

        return types;
    }

    public List<TechType> GetLockedUpgrades()
    {
        List<TechType> lockedTechs = new();

        foreach (var item in ownedTechTypes)
        {
            if (!KnownTech.Contains(item.TechType))
            {
                lockedTechs.Add(item.TechType);
            }
        }

        return lockedTechs;
    }

    public List<TechType> GetUnlockedUpgrades()
    {
        List<TechType> unlockedTechs = new();

        foreach (var item in ownedTechTypes)
        {
            if (KnownTech.Contains(item.TechType))
            {
                unlockedTechs.Add(item.TechType);
            }
        }

        return unlockedTechs;
    }

    public string GetName()
    {
        return Language.main.Get(localizationKey);
    }
}
