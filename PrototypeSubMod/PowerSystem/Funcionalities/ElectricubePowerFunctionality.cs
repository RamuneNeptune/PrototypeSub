using PrototypeSubMod.Monobehaviors;
using UnityEngine;

namespace PrototypeSubMod.PowerSystem.Funcionalities;

internal class ElectricubePowerFunctionality : PowerSourceFunctionality
{
    private static readonly Color ElectricubeLightColor = new Color(219 / 255f, 191 / 255f, 255 / 255f);

    private LightColorHandler colorHandler;

    public override void OnAbilityActivated()
    {
        var root = GetComponentInParent<SubRoot>();
        colorHandler = root.GetComponentInChildren<LightColorHandler>();

        colorHandler.SetTempColor(ElectricubeLightColor);
    }

    protected override void OnAbilityStopped()
    {
        colorHandler.ResetColor();
    }
}
