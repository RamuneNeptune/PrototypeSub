using PrototypeSubMod.PowerSystem;
using SubLibrary.SaveData;
using System.Linq;
using UnityEngine;

namespace PrototypeSubMod.SubTerminal;

internal class ProtoBuildTerminal : MonoBehaviour
{
    public bool HasBuiltProtoSub
    {
        get
        {
            return prototypeSub != null;
        }
    }

    private GameObject prototypeSub;

    private void Start()
    {
        var serializationManagers = FindObjectsOfType<SubSerializationManager>();
        prototypeSub = serializationManagers.FirstOrDefault(s => s.GetComponentInChildren<PrototypePowerSystem>() != null)?.gameObject;
    }
}
