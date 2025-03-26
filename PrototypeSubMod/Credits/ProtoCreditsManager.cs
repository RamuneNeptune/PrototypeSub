using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PrototypeSubMod.Credits;

internal class ProtoCreditsManager : MonoBehaviour
{
    [SerializeField] private RectTransform creditsMask;
    [SerializeField] private RectTransform creditsTextRect;
    [SerializeField] private TextMeshProUGUI creditsText;
    [SerializeField] private float creditsLength;
    
    private float creditsSpeed;
    private float currentCreditsLength;
    private bool loadedMainMenu;
    private bool initialized;
    
    private void Start()
    {
        creditsText.text = Language.main.Get("ProtoCreditsText");
        Canvas.ForceUpdateCanvases();
        var sizeFitter = creditsText.GetComponent<ContentSizeFitter>();
        sizeFitter.enabled = false;
        sizeFitter.enabled = true;
        
        float maskYHeight = creditsMask.rect.height;
        float textYHeight = creditsTextRect.sizeDelta.y;
        float yOffset = -(maskYHeight / 2) - (textYHeight / 2);
        creditsTextRect.localPosition = new Vector3(0, yOffset, 0);
        
        creditsSpeed = (textYHeight + maskYHeight) / creditsLength;
        creditsMask.gameObject.SetActive(true);
        
        initialized = true;
    }

    private void Update()
    {
        if (!initialized) return;
        
        if (currentCreditsLength < creditsLength)
        {
            currentCreditsLength += Time.deltaTime;
            creditsTextRect.localPosition += new Vector3(0, creditsSpeed * Time.deltaTime, 0);
        }
        else if (!loadedMainMenu)
        {
            StartCoroutine(LoadMainMenu());
            loadedMainMenu = true;
        }
    }

    private IEnumerator LoadMainMenu()
    {
        yield return new WaitForSeconds(1);
        SceneCleaner.Open();
    }
}
