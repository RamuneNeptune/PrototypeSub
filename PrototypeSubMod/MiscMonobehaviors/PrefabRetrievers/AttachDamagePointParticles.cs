using SubLibrary.Handlers;
using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.PrefabRetrievers;

internal class AttachDamagePointParticles : MonoBehaviour
{
    [Tooltip("Leave as -1 to pick a random particle prefab")]
    [SerializeField] private int fxIndex = -1;

    private void Awake()
    {
        var manager = CyclopsReferenceHandler.CyclopsReference.GetComponentInChildren<CyclopsExternalDamageManager>();
        var index = fxIndex == -1 ? Random.Range(0, manager.fxPrefabs.Length - 1) : fxIndex;
        var prefab = manager.fxPrefabs[index];

        var effect = Instantiate(prefab, transform, false);
        effect.transform.rotation = transform.rotation;
        effect.transform.position = transform.position;
    }
}
