using PrototypeSubMod.SaveData;
using PrototypeSubMod.VariablePowerStreams;
using SubLibrary.SaveData;
using UnityEngine;

namespace PrototypeSubMod.PowerSystem;

internal abstract class PowerSourceFunctionality : MonoBehaviour, ISaveDataListener
{
    private float currentTime;
    private SubSerializationManager serializationManager;

    private void OnEnable()
    {
        serializationManager = GetComponentInParent<SubSerializationManager>();
        var powerStreams = GetComponentInParent<SubRoot>().GetComponentInChildren<ProtoVariablePowerStreams>();
        currentTime = powerStreams.GetApplicableDuration();

        OnAbilityActivated();
    }

    private void Update()
    {
        currentTime -= Time.deltaTime;
        if (currentTime <= 0)
        {
            serializationManager.saveData.EnsureAsPrototypeData().currentPowerEffectDuration = -1f;
            Destroy(this);
        }
    }

    public void OnBeforeDataSaved(ref BaseSubDataClass saveData)
    {
        var protoData = saveData.EnsureAsPrototypeData();
        protoData.currentPowerEffectDuration = currentTime;
        protoData.installedPowerUpgradeType = GetType();

        saveData = protoData;
    }

    public void OnSaveDataLoaded(BaseSubDataClass saveData)
    {
        currentTime = saveData.EnsureAsPrototypeData().currentPowerEffectDuration;
    }

    private void OnDestroy()
    {
        serializationManager.saveData.EnsureAsPrototypeData().installedPowerUpgradeType = null;
        OnAbilityStopped();
    }

    public void SetTime(float time) => currentTime = time;

    public abstract void OnAbilityActivated();
    protected abstract void OnAbilityStopped();
}
