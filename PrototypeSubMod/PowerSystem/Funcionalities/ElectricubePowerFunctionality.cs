using PrototypeSubMod.Monobehaviors;
using UnityEngine;

namespace PrototypeSubMod.PowerSystem.Funcionalities;

internal class ElectricubePowerFunctionality : PowerSourceFunctionality
{
    private static readonly Color ElectricubeLightColor = new Color(219 / 255f, 191 / 255f, 255 / 255f);

    private LightColorHandler colorHandler;

    private void Start()
    {
        var root = GetComponentInParent<SubRoot>();
        colorHandler = root.GetComponentInChildren<LightColorHandler>();

        colorHandler.SetTempColor(ElectricubeLightColor);
    }

    private void OnDestroy()
    {
        colorHandler.ResetColor();
    }
}
