using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PrototypeSubMod.MiscMonobehaviors.SubSystems;

public class SFXButton : Button
{
    [SerializeField]
    public UnityEvent onClickWrapper;
    public FMODAsset onEnterFX;
    public FMODAsset onExitFX;
    public FMODAsset onClickFX;
    public float volume = 1;
    public float minDistForSound = 2;

    private bool wasOutOfRange;
    private bool mouseOnObject;
    
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
        OnPointerExit(new PointerEventData(EventSystem.current));
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (!gameObject.activeSelf) return;
        mouseOnObject = true;
        
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
        base.OnPointerExit(new PointerEventData(EventSystem.current));
        
        if (!gameObject.activeSelf) return;
        mouseOnObject = false;
        
        if ((Player.main.transform.position - transform.position).sqrMagnitude > minDistForSound * minDistForSound)
        {
            return;
        }
        
        if (onExitFX != null)
        {
            FMODUWE.PlayOneShot(onExitFX, transform.position, volume);
        }
    }
    
    public override void OnPointerClick(PointerEventData eventData) { }

    private void OnClick()
    {
        if (!gameObject.activeSelf) return;

        if ((Player.main.transform.position - transform.position).sqrMagnitude >
            minDistForSound * minDistForSound) return;

        base.OnPointerClick(new PointerEventData(EventSystem.current));

        if (onClickFX != null)
        {
            FMODUWE.PlayOneShot(onClickFX, transform.position, volume);
        }
    }
    
    private void OnUpdate()
    {
        if (mouseOnObject && GameInput.GetButtonDown(GameInput.Button.LeftHand))
        {
            OnClick();
        }
    }
    
    private IEnumerator UpdateHoverDistance()
    {
        while (gameObject.activeInHierarchy)
        {
            yield return new WaitForSeconds(1);
            bool outOfRange = (Player.main.transform.position - transform.position).sqrMagnitude >
                              minDistForSound * minDistForSound;
            
            if (wasOutOfRange == outOfRange) continue;
            
            if (outOfRange)
            {
                ((Image)targetGraphic).overrideSprite = null;
            }
            else if (mouseOnObject)
            {
                OnPointerEnter(new PointerEventData(EventSystem.current));
            }
            
            wasOutOfRange = outOfRange;
        }
    }

    private void OnEnable()
    {
        StartCoroutine(UpdateHoverDistance());
        ManagedUpdate.Subscribe(ManagedUpdate.Queue.UpdateAfterInput, OnUpdate);
    }

    private void OnDisable()
    {
        OnPointerExit(new PointerEventData(EventSystem.current));
        ManagedUpdate.Unsubscribe(ManagedUpdate.Queue.UpdateAfterInput, OnUpdate);
    }
}