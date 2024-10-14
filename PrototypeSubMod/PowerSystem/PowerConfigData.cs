using System;

namespace PrototypeSubMod.PowerSystem;

internal class PowerConfigData
{
    public float powerValue;
    public Type sourceEffectFunctionality;

    public PowerConfigData(float powerValue, Type sourceEffectFunctionality)
    {
        this.powerValue = powerValue;
        this.sourceEffectFunctionality = sourceEffectFunctionality;
    }
}
