using System.Collections;
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
        onClick.AddListener(() =>
        {
            if ((Player.main.transform.position - transform.position).sqrMagnitude >
                minDistForSound * minDistForSound) return;
            
            onClickWrapper?.Invoke();
        });
    }

    private void Start()
    {
        StartCoroutine(UpdateHoverDistance());
    }
    
    public override void OnPointerEnter(PointerEventData eventData)
    {
        if ((Player.main.transform.position - transform.position).sqrMagnitude >
            minDistForSound * minDistForSound) return;
        
        base.OnPointerEnter(eventData);
        
        if (onEnterFX != null)
        {
            FMODUWE.PlayOneShot(onEnterFX, transform.position, volume);
        }
    }
    
    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        
        if ((Player.main.transform.position - transform.position).sqrMagnitude > minDistForSound * minDistForSound)
        {
            return;
        }
        
        if (onExitFX != null)
        {
            FMODUWE.PlayOneShot(onExitFX, transform.position, volume);
        }
    }

    private IEnumerator UpdateHoverDistance()
    {
        while (gameObject.activeInHierarchy)
        {
            yield return new WaitForSeconds(1);
            bool outOfRange = (Player.main.transform.position - transform.position).sqrMagnitude >
                              minDistForSound * minDistForSound;
            if (outOfRange)
            {
                ((Image)targetGraphic).overrideSprite = null;
            }
        }
    }

    private void OnEnable()
    {
        StartCoroutine(UpdateHoverDistance());
    }
}