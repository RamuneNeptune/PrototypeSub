using PrototypeSubMod.IonGenerator;
using PrototypeSubMod.Upgrades;
using System.Linq;
using UnityEngine;

namespace PrototypeSubMod.IonBarrier;

internal class ProtoIonBarrier : ProtoUpgrade, IOnTakeDamage
{
    [SerializeField] private Renderer[] shieldRenderers;
    [SerializeField] private PowerRelay powerRelay;
    [SerializeField] private GameObject lavaLarvaeRoot;
    [SerializeField] private ProtoIonGenerator ionGenerator;
    [SerializeField] private float constantPowerDraw;
    [SerializeField] private float powerPerDamage;
    [SerializeField] private float maxShieldIntensity;
    [SerializeField] private float defaultReduction;
    [SerializeField] private DamageReductor[] damageReductors;
    [SerializeField] private Animator hydrolockController;
    [SerializeField] private VoiceNotification shieldsUpNotification;

    [SerializeField, HideInInspector] private float[] multipliers;
    [SerializeField, HideInInspector] private DamageType[] damageTypes;

    private DamageReductor[] serializedDamageReductors;
    private SubRoot subRoot;
    private float targetShieldIntensity;
    private float currentShieldIntensity;
    private float currentImpactIntensity;
    private float damageReductionMultipier;

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

        subRoot = GetComponentInParent<SubRoot>();
    }

    private void Update()
    {
        if (subRoot.LOD.IsMinimal()) return;

        HandleShieldProperties();
        HandlePowerDraw();
    }

    private void HandleShieldProperties()
    {
        currentShieldIntensity = Mathf.MoveTowards(currentShieldIntensity, targetShieldIntensity, Time.deltaTime / 2f);
        currentImpactIntensity = Mathf.MoveTowards(currentImpactIntensity, 0, Time.deltaTime / 4f);
        foreach (var rend in shieldRenderers)
        {
            rend.material.SetFloat(ShaderPropertyID._Intensity, currentShieldIntensity);
            rend.material.SetFloat(ShaderPropertyID._ImpactIntensity, currentImpactIntensity);

            if (Mathf.Approximately(currentShieldIntensity, 0) && targetShieldIntensity == 0)
            {
                rend.gameObject.SetActive(false);
            }
        }
    }

    private void HandlePowerDraw()
    {
        if (!upgradeEnabled || !upgradeInstalled) return;

        powerRelay.ConsumeEnergy(constantPowerDraw * Time.deltaTime, out _);
    }

    public float GetReductionForType(DamageType type)
    {
        DamageReductor reductor = serializedDamageReductors.FirstOrDefault(r => r.type == type);

        float multiplier = damageReductionMultipier <= 0 ? 1 : damageReductionMultipier;

        if (reductor == null)
        {
            return defaultReduction * multiplier;
        }

        return reductor.reductionMultiplier * multiplier;
    }

    public void OnTakeDamage(DamageInfo damageInfo)
    {
        if (!upgradeEnabled || !upgradeInstalled) return;

        float powerCost = damageInfo.originalDamage * powerPerDamage;
        powerRelay.ConsumeEnergy(powerCost, out _);

        foreach (var rend in shieldRenderers)
        {
            if (!rend.gameObject.activeSelf) continue;

            rend.material.SetVector(ShaderPropertyID._ImpactPosition, damageInfo.position);
            currentImpactIntensity = 1;

            if (damageInfo.dealer != null && damageInfo.dealer.GetComponent<LiveMixin>())
            {
                damageInfo.dealer.GetComponent<LiveMixin>().TakeDamage(20f, type: DamageType.Electrical);
            }
        }
    }

    public override void SetUpgradeEnabled(bool enabled)
    {
        if (upgradeLocked || !upgradeInstalled) return;

        if (ionGenerator.GetUpgradeEnabled() && upgradeEnabled == false) return;

        if (enabled && !upgradeEnabled)
        {
            ActivateShield();
        }

        if (!enabled && upgradeEnabled)
        {
            DeactivateShield();
        }

        base.SetUpgradeEnabled(enabled);
        hydrolockController.SetBool("HydrolockEnabled", enabled);
        if (enabled)
        {
            subRoot.voiceNotificationManager.PlayVoiceNotification(shieldsUpNotification);
            foreach (var larva in lavaLarvaeRoot.GetComponentsInChildren<LavaLarva>())
            {
                larva.GetComponent<LiveMixin>().TakeDamage(1, type: DamageType.Electrical);
            }
        }
    }

    private void ActivateShield()
    {
        foreach (var lavaLarva in subRoot.GetComponentsInChildren<LavaLarva>())
        {
            lavaLarva.GetComponent<LiveMixin>().TakeDamage(1, type: DamageType.Electrical);
        }

        foreach (var rend in shieldRenderers)
        {
            rend.gameObject.SetActive(true);
        }

        targetShieldIntensity = maxShieldIntensity;
    }

    private void DeactivateShield()
    {
        targetShieldIntensity = 0;
    }

    public void SetDamageReductionMultiplier(float multiplier)
    {
        damageReductionMultipier = multiplier;
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