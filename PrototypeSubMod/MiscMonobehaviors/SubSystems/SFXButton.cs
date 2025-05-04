using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PrototypeSubMod.MiscMonobehaviors.SubSystems;

public class SFXButton : Button
{
    public UnityEvent onClickWrapper;
    public FMODAsset soundEffect;
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
        
        if (soundEffect != null)
        {
            FMODUWE.PlayOneShot(soundEffect, transform.position, volume);
        }
    }
}