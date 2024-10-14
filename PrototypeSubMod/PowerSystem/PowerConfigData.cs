using System;

namespace PrototypeSubMod.PowerSystem;

internal class PowerConfigData
{
    public float powerValue;
    public PowerSourceFunctionality sourceEffectFunctionality;

    public PowerConfigData(float powerValue, PowerSourceFunctionality sourceEffectFunctionality)
    {
        this.powerValue = powerValue;
        this.sourceEffectFunctionality = sourceEffectFunctionality;
    }
}
