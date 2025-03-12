using System.Collections;
using UnityEngine;

namespace PrototypeSubMod.IonGenerator;

internal class EmpSpawner : MonoBehaviour
{
    [SerializeField] private Transform empSpawnPos;
    [SerializeField] private float empLifetime;
    [SerializeField] private AnimationCurve blastRadius;
    [SerializeField] private AnimationCurve blastHeight;

    private GameObject empPrefab;

    private IEnumerator Start()
    {
        CoroutineTask<GameObject> crabsquidTask = CraftData.GetPrefabForTechTypeAsync(TechType.CrabSquid);

        yield return crabsquidTask;

        GameObject crabsquid = crabsquidTask.result.Get();
        var empAttack = crabsquid.GetComponent<EMPAttack>();

        empAttack.ammoPrefab.SetActive(false);
        empPrefab = Instantiate(empAttack.ammoPrefab);

        empAttack.ammoPrefab.SetActive(true);
        var empBlast = empPrefab.GetComponent<EMPBlast>();

        empBlast.lifeTime = empLifetime;
        empBlast.blastRadius = blastRadius;
        empBlast.blastHeight = blastHeight;
    }

    public void FireEMP(float disableElectronicsTime)
    {
        var newEMP = Instantiate(empPrefab, empSpawnPos.position, empSpawnPos.rotation);
        newEMP.SetActive(true);
        newEMP.GetComponent<EMPBlast>().disableElectronicsTime = disableElectronicsTime;
    }

    public Transform GetSpawnPos() => empSpawnPos;
}
