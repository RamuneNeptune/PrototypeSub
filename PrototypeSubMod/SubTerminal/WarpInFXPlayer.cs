using System.Collections;
using UnityEngine;

namespace PrototypeSubMod.SubTerminal;

public class WarpInFXPlayer : MonoBehaviour
{
    private GameObject warpInFX;

    private void Start()
    {
        UWE.CoroutineHost.StartCoroutine(RetrievePrefab());
    }
    
    private IEnumerator RetrievePrefab()
    {
        var task = CraftData.GetPrefabForTechTypeAsync(TechType.Warper);
        yield return task;

        var result = task.GetResult();
        var warper = result.GetComponent<Warper>();
        warpInFX = warper.warpInEffectPrefab;
    }

    public void SpawnWarpInFX(Vector3 position, Vector3 scale)
    {
        var fx = Instantiate(warpInFX, position, Quaternion.identity);
        fx.transform.localScale = scale;
    }
}