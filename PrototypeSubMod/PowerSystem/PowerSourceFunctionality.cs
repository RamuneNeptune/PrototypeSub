using PrototypeSubMod.SaveData;
using PrototypeSubMod.VariablePowerStreams;
using SubLibrary.SaveData;
using UnityEngine;

namespace PrototypeSubMod.PowerSystem;

public abstract class PowerSourceFunctionality : MonoBehaviour, ISaveDataListener
{
    protected float currentTime;
    protected SubSerializationManager serializationManager;

    private void OnEnable()
    {
        serializationManager = GetComponentInParent<SubSerializationManager>();
        currentTime = 120;

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
    public float GetTimeLeft() => currentTime;

    public abstract void OnAbilityActivated();
    protected abstract void OnAbilityStopped();
}
