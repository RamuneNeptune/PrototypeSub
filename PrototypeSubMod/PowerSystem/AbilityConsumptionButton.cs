using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PrototypeSubMod.PowerSystem;

internal class AbilityConsumptionButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Button button;
    [SerializeField] private Image progressBar;
    [SerializeField] private Animator animator;
    [SerializeField] private float confirmTime; 

    private float currentConfirmTime;
    private bool clicking;
    private bool consumed;

    private void Start()
    {
        animator.speed = 1 / confirmTime;
    }

    private void Update()
    {
        if (!clicking || !button.interactable)
        {
            currentConfirmTime = 0;
            consumed = false;
            return;
        }

        progressBar.fillAmount = currentConfirmTime / confirmTime;
        currentConfirmTime += Time.deltaTime;
        if (currentConfirmTime >= confirmTime && !consumed)
        {
            consumed = true;
            progressBar.fillAmount = 0;
            ProtoPowerAbilitySystem.Instance.ConsumeItem();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        clicking = true;
        animator.SetBool("Charging", true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        clicking = false;
        animator.SetBool("Charging", false);
    }

    private void OnEnable()
    {
        if (ProtoPowerAbilitySystem.Instance == null)
        {
            button.interactable = false;
            return;
        }

        button.interactable = ProtoPowerAbilitySystem.Instance.HasItem();
        ProtoPowerAbilitySystem.Instance.onEquip += EnableButton;
        ProtoPowerAbilitySystem.Instance.onUnequip += DisableButton;
    }

    private void OnDisable()
    {
        if (ProtoPowerAbilitySystem.Instance == null) return;

        ProtoPowerAbilitySystem.Instance.onEquip -= EnableButton;
        ProtoPowerAbilitySystem.Instance.onUnequip -= DisableButton;
    }

    private void EnableButton()
    {
        button.interactable = true;
        button.image.color = Color.white;
        progressBar.fillAmount = 0;
    }

    private void DisableButton()
    {
        button.interactable = false;
        button.image.color = new Color(0.75f, 0.75f, 0.75f);
        progressBar.fillAmount = 0;
    }
}
