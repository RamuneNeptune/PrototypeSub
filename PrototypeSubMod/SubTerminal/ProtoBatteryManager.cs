using UnityEngine;

namespace PrototypeSubMod.SubTerminal;

internal class ProtoBatteryManager : MonoBehaviour
{
    [SerializeField] private Renderer sliderRend;

    private float currentDrainTime;
    private float drainTime;

    private float currentChargeTime;
    private float chargeTime;
    private bool charging;

    private void Start()
    {
        sliderRend.material.SetFloat("_FilledAmount", 0);
    }

    public void StartBatteryDrain(float duration)
    {
        currentDrainTime = duration;
        drainTime = duration;
    }

    public void StartBatteryCharge(float duration)
    {
        charging = true;
        chargeTime = duration;
        currentChargeTime = 0;
    }

    private void Update()
    {
        HandleCharge();
        HandleDrain();
    }

    private void HandleCharge()
    {
        if (charging && currentChargeTime < chargeTime)
        {
            currentChargeTime += Time.deltaTime;
            sliderRend.material.SetFloat("_FilledAmount", currentChargeTime / chargeTime);
        }
        else if (currentChargeTime >= chargeTime)
        {
            charging = false;
        }
    }

    private void HandleDrain()
    {
        if (currentDrainTime > 0 && !charging)
        {
            currentDrainTime -= Time.deltaTime;
            sliderRend.material.SetFloat("_FilledAmount", currentDrainTime / drainTime);
        }
    }
}
