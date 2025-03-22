using System;
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
    
    private void Start()
    {
        float maskYHeight = creditsMask.rect.height;
        float textYHeight = creditsText.rect.height;
        float yOffset = -maskYHeight / 2 - textYHeight / 2;
        creditsText.localPosition = new Vector3(0, yOffset, 0);

        creditsLength = Mathf.Abs(yOffset * 2) / creditsSpeed;
        
        
    }

    private void Update()
    {
        if (currentCreditsLength < creditsLength)
        {
            currentCreditsLength += Time.deltaTime;
        }
        else
        {
            // Credits complete
        }
        
        creditsText.localPosition += new Vector3(0, creditsSpeed * Time.deltaTime, 0);
    }
}
