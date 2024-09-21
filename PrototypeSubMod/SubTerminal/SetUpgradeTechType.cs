using System;
using UnityEngine;

namespace PrototypeSubMod.SubTerminal;

internal class SetUpgradeTechType : MonoBehaviour
{
    [SerializeField] private string upgradeTechType;

    private void Start()
    {
        TechType techType = (TechType)Enum.Parse(typeof(TechType), upgradeTechType);
        GetComponentInParent<uGUI_ProtoUpgradeIcon>().SetUpgradeTechType(techType);
    }
}
