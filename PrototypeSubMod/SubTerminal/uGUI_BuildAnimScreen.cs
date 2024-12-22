using UnityEngine;
using UnityEngine.UI;

namespace PrototypeSubMod.SubTerminal;

internal class uGUI_BuildAnimScreen : MonoBehaviour
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
