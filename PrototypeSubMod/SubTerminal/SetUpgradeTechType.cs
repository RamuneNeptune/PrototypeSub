using System;
using UnityEngine;

namespace PrototypeSubMod.SubTerminal;

[RequireComponent(typeof(uGUI_ProtoUpgradeIcon))]
internal class SetUpgradeTechType : MonoBehaviour
{
    [SerializeField] private string upgradeTechType;

    private void Start()
    {
        TechType techType = (TechType)Enum.Parse(typeof(TechType), upgradeTechType);
        GetComponent<uGUI_ProtoUpgradeIcon>().SetUpgradeTechType(techType);
    }
}
