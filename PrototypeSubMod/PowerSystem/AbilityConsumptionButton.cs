using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PrototypeSubMod.PowerSystem;

internal class AbilityConsumptionButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, ISelectable
{
    [SerializeField] private RectTransform rect;
    [SerializeField] private uGUI_EquipmentSlot dummySlot;
    [SerializeField] private Image progressBar;
    [SerializeField] private Image mainGraphic;
    [SerializeField] private Animator animator;
    [SerializeField] private FMOD_CustomEmitter chargeEmitter;
    [SerializeField] private CanvasGroup buttonGroup;
    [SerializeField] private float confirmTime;

    private float currentConfirmTime;
    private bool clicking;
    private bool hovering;
    private bool consumed;
    private bool interactable;

    private bool checkForControllerHold;

    private ProtoPowerAbilitySystem activeAbilitySystem;

    private void Start()
    {
        animator.speed = 1 / confirmTime;
        progressBar.fillAmount = 0;

        DisableButton();
    }

    private void Update()
    {
        CheckForControllerInput();
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

    private void CheckForControllerInput()
    {
        if (!checkForControllerHold) return;

        checkForControllerHold = GameInput.GetButtonHeld(GameInput.Button.UISubmit);
        if (!checkForControllerHold)
        {
            OnStopPress();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnStartPress();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnStopPress();
    }

    private void OnStartPress()
    {
        clicking = true;

        if (interactable)
        {
            animator.SetBool("Charging", true);
            chargeEmitter.Play();
        }
    }

    private void OnStopPress()
    {
        clicking = false;
        chargeEmitter.Stop();
    }

    public Graphic GetGraphic() => mainGraphic;
    public uGUI_EquipmentSlot GetDummySlot() => dummySlot;

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
        buttonGroup.alpha = 1f;
        progressBar.fillAmount = 0;
    }

    private void DisableButton()
    {
        interactable = false;
        buttonGroup.alpha = 0.5f;
        progressBar.fillAmount = 0;
    }

    private void UpdateButtonSprite()
    {
        float opacity = 1;
        if (hovering) opacity = 0.9f;
        if (clicking) opacity = 0.8f;
        if (!interactable) opacity = 0.5f;

        buttonGroup.alpha = opacity;
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

    public bool IsValid()
    {
        return this != null && isActiveAndEnabled;
    }

    public RectTransform GetRect()
    {
        return rect;
    }

    public bool OnButtonDown(GameInput.Button button)
    {
        if (button == GameInput.Button.UISubmit)
        {
            checkForControllerHold = true;
            OnStartPress();
            return true;
        }

        return false;
    }
}
