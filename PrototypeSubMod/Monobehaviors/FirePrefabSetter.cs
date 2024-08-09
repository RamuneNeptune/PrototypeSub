using SubLibrary.CyclopsReferencers;
using UnityEngine;

namespace PrototypeSubMod.Monobehaviors;

internal class FirePrefabSetter : MonoBehaviour, ICyclopsReferencer
{
    [SerializeField] private string firePrefabString;
    [SerializeField] private PrefabSpawn prefabSpawn;

    private void OnValidate()
    {
        if (!prefabSpawn) TryGetComponent(out prefabSpawn);
    }

    public void OnCyclopsReferenceFinished(GameObject cyclops)
    {
        prefabSpawn.prefab = cyclops.transform.Find(firePrefabString).GetComponent<PrefabSpawn>().prefab;
    }
}
