using System;

namespace PrototypeSubMod.PowerSystem;

internal class PowerConfigData
{
    public float powerValue;
    public Type sourceEffectType;

    public PowerConfigData(float powerValue, Type sourceEffectType)
    {
        this.powerValue = powerValue;
        this.sourceEffectType = sourceEffectType;
    }
}
