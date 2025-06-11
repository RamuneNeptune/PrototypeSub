using SubLibrary.Monobehaviors;
using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.Materials;

public class PrefabModifierCaller : MonoBehaviour
{
    private void Awake()
    {
        foreach (var modifier in gameObject.GetComponentsInChildren<PrefabModifier>(true))
        {
            modifier.OnAsyncPrefabTasksCompleted();
            modifier.OnLateMaterialOperation();
        }
    }
}