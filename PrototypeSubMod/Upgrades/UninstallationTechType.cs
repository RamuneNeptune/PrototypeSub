using PrototypeSubMod.Utility;
using UnityEngine;

namespace PrototypeSubMod.Upgrades;

[CreateAssetMenu(fileName = "UninstallationTechType", menuName = "Prototype Sub/Uninstallation Tech Type")]
internal class UninstallationTechType : ScriptableObject
{
    public DummyTechType ownerType;
    public Sprite sprite;
    public string nameSuffix = "_Uninstalled";
}
