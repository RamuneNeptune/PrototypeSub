using PrototypeSubMod.SaveData;
using PrototypeSubMod.VariablePowerStreams;
using SubLibrary.SaveData;
using UnityEngine;

namespace PrototypeSubMod.PowerSystem;

internal class PowerSourceFunctionality : MonoBehaviour, ISaveDataListener
{
    private float currentTime;
    private SubSerializationManager serializationManager;

    private void OnEnable()
    {
        serializationManager = GetComponentInParent<SubSerializationManager>();
        var powerStreams = GetComponentInParent<SubRoot>().GetComponentInChildren<ProtoVariablePowerStreams>();
        currentTime = powerStreams.GetApplicableDuration();
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
        saveData.EnsureAsPrototypeData().currentPowerEffectDuration = currentTime;
    }

    public void OnSaveDataLoaded(BaseSubDataClass saveData)
    {
        currentTime = saveData.EnsureAsPrototypeData().currentPowerEffectDuration;
    }
}
