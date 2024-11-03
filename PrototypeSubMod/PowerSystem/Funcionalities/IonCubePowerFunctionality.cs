using PrototypeSubMod.IonGenerator;

namespace PrototypeSubMod.PowerSystem.Funcionalities;

internal class IonCubePowerFunctionality : PowerSourceFunctionality
{
    private const float MAX_HEALTH_MULTIPLIER = 1.1f;
    private const float GENERATOR_ENERGY_MULTIPLIER = 1.15f;

    private LiveMixin mixin;
    private ProtoIonGenerator ionGenerator;
    private float originalMaxHealth;

    public override void OnAbilityActivated()
    {
        mixin = GetComponentInParent<LiveMixin>();
        originalMaxHealth = mixin.data.maxHealth;

        float fraction = mixin.GetHealthFraction();

        mixin.data.maxHealth *= MAX_HEALTH_MULTIPLIER;
        mixin.health = fraction * mixin.maxHealth;

        ionGenerator = mixin.gameObject.GetComponentInChildren<ProtoIonGenerator>();
        ionGenerator.SetEnergyMultiplier(GENERATOR_ENERGY_MULTIPLIER);
    }

    protected override void OnAbilityStopped()
    {
        float currentFraction = mixin.GetHealthFraction();
        mixin.data.maxHealth = originalMaxHealth;
        mixin.health = currentFraction * mixin.maxHealth;

        ionGenerator.SetEnergyMultiplier(1f);
    }
}
