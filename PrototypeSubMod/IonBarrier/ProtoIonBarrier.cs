using PrototypeSubMod.Upgrades;
using System.Linq;
using UnityEngine;

namespace PrototypeSubMod.IonBarrier;

internal class ProtoIonBarrier : ProtoUpgrade, IOnTakeDamage
{
    [SerializeField] private PowerRelay powerRelay;
    [SerializeField] private float constantPowerDraw;
    [SerializeField] private float powerPerDamage;
    [SerializeField] private float defaultReduction;
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

    private void Update()
    {
        if (!upgradeEnabled || !upgradeInstalled) return;

        powerRelay.ConsumeEnergy(constantPowerDraw * Time.deltaTime, out _);
    }

    public float GetReductionForType(DamageType type)
    {
        DamageReductor reductor = serializedDamageReductors.FirstOrDefault(r => r.type == type);

        if (reductor == null)
        {
            return defaultReduction;
        }

        return reductor.reductionMultiplier;
    }

    public void OnTakeDamage(DamageInfo damageInfo)
    {
        float powerCost = damageInfo.originalDamage * powerPerDamage;
        powerRelay.ConsumeEnergy(powerCost, out _);
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