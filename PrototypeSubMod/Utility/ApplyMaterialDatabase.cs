using UnityEngine;

namespace PrototypeSubMod.Utility;

public class ApplyMaterialDatabase : MonoBehaviour
{
    [SerializeField] private GameObject applyTo;

    private void OnValidate()
    {
        if (!applyTo) applyTo = gameObject;
    }

    private void Start()
    {
        if (applyTo == null) applyTo = gameObject;
        
        UWE.CoroutineHost.StartCoroutine(ProtoMatDatabase.ReplaceVanillaMats(applyTo));
    }
}