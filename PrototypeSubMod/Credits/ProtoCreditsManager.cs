using System;
using PrototypeSubMod.Utility;
using UnityEngine;

namespace PrototypeSubMod.Credits;

internal class ProtoCreditsManager : MonoBehaviour
{
    [SaveStateReference]
    public static ProtoCreditsManager instance;
    
    [SerializeField] private ProtoScreenFadeManager fadeManager;
    [SerializeField] private RectTransform creditsMask;
    [SerializeField] private RectTransform creditsText;
    [SerializeField] private float creditsSpeed;
    
    private float creditsLength;
    private bool creditsActive;
    private float currentCreditsLength;

    private void Awake()
    {
        if (instance != null) throw new Exception("More than one ProtoCreditsManager in scene.");
        
        instance = this;
    }
    
    private void Start()
    {
        float yHeight = creditsText.rect.y;
        creditsText.localPosition = new Vector3(0, -yHeight, 0);

        creditsLength = yHeight / creditsMask.rect.y * creditsSpeed;
        
        creditsMask.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!creditsActive) return;

        if (currentCreditsLength < creditsLength)
        {
            currentCreditsLength += Time.deltaTime;
        }
        else
        {
            creditsActive = false;
        }
        
        creditsText.localPosition += new Vector3(0, creditsSpeed * Time.deltaTime, 0);
    }

    public void ShowCredits()
    {
        gameObject.SetActive(true);
        creditsActive = true;
    }
    
    public ProtoScreenFadeManager GetFadeManager() => fadeManager;
}
