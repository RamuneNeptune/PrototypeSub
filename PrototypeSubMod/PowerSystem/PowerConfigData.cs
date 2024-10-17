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
    /// <param name="powerValue"></param>
    /// <param name="sourceEffectFunctionality">This type must inherit from <see cref="PowerSourceFunctionality"/>></param>
    public PowerConfigData(float powerValue, Type sourceEffectFunctionality)
    {
        this.powerValue = powerValue;
        _sourceEffectFunctionality = sourceEffectFunctionality.IsSubclassOf(typeof(PowerSourceFunctionality)) ? sourceEffectFunctionality : null;
    }
}
