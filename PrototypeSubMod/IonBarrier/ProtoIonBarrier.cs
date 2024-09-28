using PrototypeSubMod.Upgrades;
using System.Linq;
using UnityEngine;

namespace PrototypeSubMod.IonBarrier;

internal class ProtoIonBarrier : ProtoUpgrade
{
    [SerializeField] private DamageReductor[] damageReductors;

    private float[] multipliers;
    private DamageType[] damageTypes;

    private DamageReductor[] serializedDamageReductors;

    private void OnValidate()
    {
        multipliers = new float[damageReductors.Length];
        damageTypes = new DamageType[damageReductors.Length];

        for (int i = 0; i < damageReductors.Length; i++)
        {
            multipliers[i] = damageReductors[i].reductionMultiplier;
            damageTypes[i] = damageReductors[i].type;
        }
    }

    private void Start()
    {
        serializedDamageReductors = new DamageReductor[multipliers.Length];
        for (int i = 0; i < multipliers.Length; i++)
        {
            serializedDamageReductors[i] = new(multipliers[i], damageTypes[i]);
        }
    }

    public float GetReductionForType(DamageType type)
    {
        DamageReductor reductor = serializedDamageReductors.FirstOrDefault(r => r.type == type);

        if (reductor == null)
        {
            return 1;
        }

        return reductor.reductionMultiplier;
    }
}

[System.Serializable]
internal class DamageReductor
{
    public float reductionMultiplier;
    public DamageType type;

    public DamageReductor(float reductionMultiplier, DamageType type)
    {
        this.reductionMultiplier = reductionMultiplier;
        this.type = type;
    }
}