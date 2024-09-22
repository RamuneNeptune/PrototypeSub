using System.Collections.Generic;
using UnityEngine;

namespace PrototypeSubMod.Upgrades;

internal class ProtoUpgradeManager : MonoBehaviour
{
    public static ProtoUpgradeManager Instance { get; private set; }

    private Dictionary<TechType, ProtoUpgrade> upgrades = new();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        foreach (var protoUpgrade in GetComponentsInChildren<ProtoUpgrade>(true))
        {
            upgrades.Add(protoUpgrade.techType.TechType, protoUpgrade);
        }
    }

    public void SetUpgradeActive(TechType techType, bool active)
    {

    }
}
