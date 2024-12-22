using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PrototypeSubMod.PowerSystem;

internal class AbilityConsumptionButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image buttonImage;
    [SerializeField] private Image progressBar;
    [SerializeField] private Animator animator;
    [SerializeField] private FMOD_CustomEmitter chargeEmitter;
    [SerializeField] private float confirmTime;

    [Header("Sprites")]
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite hoverSprite;
    [SerializeField] private Sprite pressedSprite;
    [SerializeField] private Sprite disabledSprite;

    private float currentConfirmTime;
    private bool clicking;
    private bool hovering;
    private bool consumed;
    private bool interactable;

    private ProtoPowerAbilitySystem activeAbilitySystem;

    private void Start()
    {
        animator.speed = 1 / confirmTime;
        progressBar.fillAmount = 0;
    }

    private void Update()
    {
        UpdateButtonSprite();

        if (!clicking || !interactable)
        {
            currentConfirmTime = 0;
            progressBar.fillAmount = 0;
            consumed = false;
            animator.SetBool("Charging", false);
            return;
        }

        progressBar.fillAmount = currentConfirmTime / confirmTime;
        currentConfirmTime += Time.deltaTime;
        if (currentConfirmTime >= confirmTime && !consumed && activeAbilitySystem != null)
        {
            consumed = true;
            progressBar.fillAmount = 0;
            activeAbilitySystem.ConsumeItem();
            animator.SetBool("Charging", false);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        clicking = true;

        if (interactable)
        {
            animator.SetBool("Charging", true);
            chargeEmitter.Play();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        clicking = false;
        chargeEmitter.Stop();
    }

    private void OnEnable()
    {
        if (activeAbilitySystem == null)
        {
            interactable = false;
            return;
        }

        interactable = activeAbilitySystem.HasItem();
        activeAbilitySystem.onEquip += EnableButton;
        activeAbilitySystem.onUnequip += DisableButton;
    }

    private void OnDisable()
    {
        if (activeAbilitySystem == null) return;

        activeAbilitySystem.onEquip -= EnableButton;
        activeAbilitySystem.onUnequip -= DisableButton;
    }

    private void EnableButton()
    {
        interactable = true;
        buttonImage.color = Color.white;
        progressBar.fillAmount = 0;
    }

    private void DisableButton()
    {
        interactable = false;
        buttonImage.color = new Color(0.75f, 0.75f, 0.75f);
        progressBar.fillAmount = 0;
    }

    private void UpdateButtonSprite()
    {
        Sprite sprite = normalSprite;
        if (hovering) sprite = hoverSprite;
        if (clicking) sprite = pressedSprite;
        if (!interactable) sprite = disabledSprite;

        buttonImage.sprite = sprite;
    }

    public void SetActiveAbilitySystem(ProtoPowerAbilitySystem system)
    {
        activeAbilitySystem = system;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovering = false;
    }

    public void OnClosePDA(PDA pda)
    {
        clicking = false;
        hovering = false;
        activeAbilitySystem = null;
    }
}
