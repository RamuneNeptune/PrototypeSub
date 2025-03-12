using System;

namespace PrototypeSubMod.Utility;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
internal class SaveStateReferenceAttribute : Attribute
{
    public object defaultValue;

    public SaveStateReferenceAttribute()
    {
        defaultValue = null;
    }

    public SaveStateReferenceAttribute(object defaultValue)
    {
        this.defaultValue = defaultValue;
    }
}
