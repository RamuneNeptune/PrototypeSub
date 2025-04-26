using PrototypeSubMod.MotorHandler;
using PrototypeSubMod.SaveData;
using SubLibrary.SaveData;
using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.SubSystems;

public class ProtoFinsManager : MonoBehaviour, ISaveDataListener
{
    [SerializeField] private GameObject[] leftFins;
    [SerializeField] private GameObject[] rightFins;
    [SerializeField] private ProtoMotorHandler motorHandler;
    [SerializeField] private float multiplierIncreasePerFin;
    [SerializeField] private float defaultSpeed;

    private int installedFinCount;

    private void Start()
    {
        UpdateFinStatus();
    }
    
    public void OnSaveDataLoaded(BaseSubDataClass saveData)
    {
        installedFinCount = saveData.EnsureAsPrototypeData().installedFinCount;
        UpdateFinStatus();
    }

    public void OnBeforeDataSaved(ref BaseSubDataClass saveData)
    {
        saveData.EnsureAsPrototypeData().installedFinCount = installedFinCount;
    }
    
    public int GetInstalledFinCount() => installedFinCount;

    public void SetInstalledFinCount(int count)
    {
        installedFinCount = count;
        UpdateFinStatus();
    }

    private void UpdateFinStatus()
    {
        for (int i = 0; i < leftFins.Length; i++)
        {
            leftFins[i].SetActive(i < installedFinCount);
            rightFins[i].SetActive(i < installedFinCount);
        }

        motorHandler.AddSpeedBonus(new ProtoMotorHandler.ValueRegistrar(this, defaultSpeed + installedFinCount * multiplierIncreasePerFin));
    }
}