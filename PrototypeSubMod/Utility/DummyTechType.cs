using System;
using UnityEngine;

namespace PrototypeSubMod.Utility;

[CreateAssetMenu(fileName = "DummyTechType", menuName = "Prototype Sub/TechType")]
internal class DummyTechType : ScriptableObject
{
    public string techTypeName;

    public TechType TechType
    {
        get
        {
            if (_techType == TechType.None)
            {
                _techType = (TechType)Enum.Parse(typeof(TechType), techTypeName);
            }

            return _techType;
        }
    }

    private TechType _techType;
}
