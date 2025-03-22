using System;
using System.Collections;
using PrototypeSubMod.Utility;
using UnityEngine;

namespace PrototypeSubMod.Credits;

internal class ProtoCreditsManager : MonoBehaviour
{
    [SerializeField] private RectTransform creditsMask;
    [SerializeField] private RectTransform creditsText;
    [SerializeField] private float creditsSpeed;
    
    private float creditsLength;
    private float currentCreditsLength;
    private bool loadedMainMenu;
    private bool initialized;
    
    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        
        float maskYHeight = creditsMask.rect.height;
        float textYHeight = creditsText.rect.height;
        float yOffset = -maskYHeight / 2 - textYHeight / 2;
        creditsText.localPosition = new Vector3(0, yOffset, 0);

        creditsLength = Mathf.Abs(yOffset * 2) / creditsSpeed;

        initialized = true;
    }

    private void Update()
    {
        if (!initialized) return;
        
        if (currentCreditsLength < creditsLength)
        {
            currentCreditsLength += Time.deltaTime;
            creditsText.localPosition += new Vector3(0, creditsSpeed * Time.deltaTime, 0);
        }
        else if (!loadedMainMenu)
        {
            //SceneCleaner.Open();
            //loadedMainMenu = true;
        }
    }
}
