using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PrototypeSubMod.MiscMonobehaviors.SubSystems;

public class SFXButton : Button
{
    public UnityEvent onClickWrapper;
    public FMODAsset onEnterFX;
    public FMODAsset onExitFX;
    public float volume = 1;
    public float minDistForSound = 2;

    private void Awake()
    {
        onClick.AddListener(() => onClickWrapper?.Invoke());
    }
    
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        if ((Player.main.transform.position - transform.position).sqrMagnitude >
            minDistForSound * minDistForSound) return;
        
        if (onEnterFX != null)
        {
            FMODUWE.PlayOneShot(onEnterFX, transform.position, volume);
        }
    }
    
    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        ((Image)targetGraphic).overrideSprite = null;
        
        if ((Player.main.transform.position - transform.position).sqrMagnitude >
            minDistForSound * minDistForSound) return;
        
        if (onExitFX != null)
        {
            FMODUWE.PlayOneShot(onExitFX, transform.position, volume);
        }
    }
}