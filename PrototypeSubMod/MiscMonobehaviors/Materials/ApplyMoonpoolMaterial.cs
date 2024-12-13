using System.Collections;
using UnityEngine;
using UWE;

namespace PrototypeSubMod.MiscMonobehaviors.Materials;

internal class ApplyMoonpoolMaterial : MonoBehaviour
{
    [SerializeField] private Renderer renderer;

    private void OnValidate()
    {
        if (!renderer) TryGetComponent(out renderer);
    }

    private void Start()
    {
        UWE.CoroutineHost.StartCoroutine(SwapMaterial());
    }

    private IEnumerator SwapMaterial()
    {
        var task = PrefabDatabase.GetPrefabAsync("e6ac95bf-4daa-440d-8ce7-4437ed47c3de");

        yield return task;

        bool success = task.TryGetPrefab(out var prefab);
        if (!success)
        {
            throw new System.Exception("Error loading moonpool surface prefab");
        }

        var surface = prefab.transform.Find("Surface");
        renderer.material = new Material(surface.GetComponent<Renderer>().material);
    }
}
