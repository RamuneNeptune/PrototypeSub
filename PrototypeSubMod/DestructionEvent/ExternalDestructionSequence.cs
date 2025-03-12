using PrototypeSubMod.IonGenerator;
using UnityEngine;

namespace PrototypeSubMod.DestructionEvent;

internal class ExternalDestructionSequence : DestructionSequence
{
    [SerializeField] private EmpSpawner empSpawner;
    [SerializeField] private float disableElectronicsTime;

    public override void StartSequence(SubRoot subRoot)
    {
        empSpawner.FireEMP(disableElectronicsTime);
        subRoot.gameObject.SetActive(false);
        subRoot.transform.position = new Vector3(0, 100, 0);
    }
}
