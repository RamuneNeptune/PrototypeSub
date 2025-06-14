using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PrototypeSubMod.SubTerminal;

internal class uGUI_BuildAnimScreen : TerminalScreen
{
    [SerializeField] private Image progressBar;

    private float duration;
    private float currentProgress;
    private bool startAnim;

    public void StartAnimation(float duration)
    {
        progressBar.fillAmount = 0;
        this.duration = duration;
        currentProgress = 0;
        startAnim = true;
    }

    public void StartPreWarm(float duration)
    {
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

    public override void OnStageStarted()
    {
        gameObject.SetActive(true);
    }

    public override void OnStageFinished()
    {
        gameObject.SetActive(false);
    }
}
