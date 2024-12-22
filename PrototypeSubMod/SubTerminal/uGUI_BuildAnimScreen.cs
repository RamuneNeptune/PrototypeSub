using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PrototypeSubMod.SubTerminal;

internal class uGUI_BuildAnimScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI displayText;
    [SerializeField] private Image progressBar;
    [SerializeField] private string chargingLocalizationKey;
    [SerializeField] private string buildingLocalizationKey;

    private float duration;
    private float currentProgress;
    private bool startAnim;

    public void StartAnimation(float duration)
    {
        progressBar.fillAmount = 0;
        this.duration = duration;
        currentProgress = 0;
        startAnim = true;

        displayText.text = Language.main.Get(buildingLocalizationKey);
    }

    public void StartPreWarm(float duration)
    {
        displayText.text = Language.main.Get(chargingLocalizationKey);

        progressBar.fillAmount = 0;
        this.duration = duration;
        currentProgress = 0;
        startAnim = true;
    }

    private void Update()
    {
        if (currentProgress < duration && startAnim)
        {
            currentProgress += Time.deltaTime;
            progressBar.fillAmount = currentProgress / duration;
        }
        else if (startAnim)
        {
            startAnim = false;
        }
    }
}
