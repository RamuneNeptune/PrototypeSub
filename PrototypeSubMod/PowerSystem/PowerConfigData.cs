using System;

namespace PrototypeSubMod.PowerSystem;

public class PowerConfigData
{
    public float powerValue;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="chargeCount"></param>
    /// <param name="sourceEffectFunctionality">This type must inherit from <see cref="PowerSourceFunctionality"/>></param>
    public PowerConfigData(int chargeCount)
    {
        powerValue = chargeCount * PrototypePowerSystem.CHARGE_POWER_AMOUNT;
    }
}
