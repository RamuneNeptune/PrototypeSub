using UnityEngine;

namespace PrototypeSubMod.SubTerminal;

internal class ProtoBatteryManager : MonoBehaviour
{
    [SerializeField] private Renderer sliderRend;
    [SerializeField] private float rechargeTime;

    private float currentDrainTime;
    private float drainTime;
    private float currentRechargeTime;

    public void StartBatteryDrain(float duration)
    {
        currentDrainTime = duration;
        drainTime = duration;
        currentRechargeTime = 0;
    }

    private void Update()
    {
        if (currentDrainTime > 0)
        {
            currentDrainTime -= Time.deltaTime;
            sliderRend.material.SetFloat("_FilledAmount", currentDrainTime / drainTime);
        }
        else if (currentRechargeTime < rechargeTime)
        {
            currentRechargeTime += Time.deltaTime;
            sliderRend.material.SetFloat("_FilledAmount", currentDrainTime / drainTime);
        }
    }
}
