using System;

namespace PrototypeSubMod.PowerSystem;

public class PowerConfigData
{
    public float powerValue;
    public Type SourceEffectFunctionality
    {
        get
        {
            return _sourceEffectFunctionality;
        }
    }

    private Type _sourceEffectFunctionality;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="chargeCount"></param>
    /// <param name="sourceEffectFunctionality">This type must inherit from <see cref="PowerSourceFunctionality"/>></param>
    public PowerConfigData(int chargeCount, Type sourceEffectFunctionality)
    {
        Plugin.Logger.LogInfo($"Creating power config data. Charge count: {chargeCount} | Power value = {chargeCount * PrototypePowerSystem.CHARGE_POWER_AMOUNT}");
        powerValue = chargeCount * PrototypePowerSystem.CHARGE_POWER_AMOUNT;
        if (sourceEffectFunctionality == null)
        {
            _sourceEffectFunctionality = null;
            return;
        }

        _sourceEffectFunctionality = sourceEffectFunctionality.IsSubclassOf(typeof(PowerSourceFunctionality)) ? sourceEffectFunctionality : null;
    }
}
