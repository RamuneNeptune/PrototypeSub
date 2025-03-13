using PrototypeSubMod.IonGenerator;
using System.Collections;
using UnityEngine;

namespace PrototypeSubMod.DestructionEvent;

internal class ExternalDestructionSequence : DestructionSequence
{
    [SerializeField] private Transform warpOutSpawnPos;
    [SerializeField] private EmpSpawner empSpawner;
    [SerializeField] private float disableElectronicsTime;

    private GameObject warpOutFX;

    private IEnumerator Start()
    {
        var task = CraftData.GetPrefabForTechTypeAsync(TechType.Warper);
        yield return task;

        var result = task.GetResult();
        var warper = result.GetComponent<Warper>();
        warpOutFX = warper.warpOutEffectPrefab;
    }

    public override void StartSequence(SubRoot subRoot)
    {
        empSpawner.FireEMP(disableElectronicsTime);
        subRoot.gameObject.SetActive(false);
        subRoot.transform.position = new Vector3(0, 100, 0);

        var fx = Instantiate(warpOutFX, warpOutSpawnPos.position, warpOutSpawnPos.rotation);
        fx.transform.localScale = Vector3.one * 15f;
    }
}
