using PrototypeSubMod.Patches;
using PrototypeSubMod.UI.ProceduralArcGenerator;
using SubLibrary.UI;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace PrototypeSubMod.UI.AbilitySelection;

public class TetherManager : MonoBehaviour, IUIElement
{
    public Action onAbilitySelected;

    [SerializeField] private PilotingChair chair;
    [SerializeField] private Transform tetherPoint;
    [SerializeField] private CircularMeshApplier selectionHighlight;
    [SerializeField] private IconDistributor distributor;
    [SerializeField] private Image selectionPreview;
    [SerializeField] private FMODAsset openSFX;
    [SerializeField] private FMODAsset hoverSFX;
    [SerializeField] private FMODAsset selectSFX;
    [SerializeField] private float tetherSensitivity;
    [SerializeField] private float tetherLength;
    [SerializeField] private float cursorAngleMultiplier;

    private RadialIcon lastIcon;
    private RadialIcon selectedIcon;

    private float lastTetherAngle;
    private float timeLastAngleCalculated;
    private bool menuOpen;
    private float timeOpened;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        selectionHighlight.gameObject.SetActive(false);

        RegenerateHighlightArc();
    }

    public void UpdateUI()
    {
        if (Time.time - 0.01f <= timeOpened) return;

        HandleActivation();

        if (!menuOpen) return;

        UpdateTetherPoint();
        UpdateIconNotifs();
        UpdateSelection();
    }

    public void OnSubDestroyed() { }

    private void UpdateTetherPoint()
    {
        Vector2 delta = MainCameraControl_Patches.GetOverwrittenLookDelta() * Time.deltaTime * tetherSensitivity;
        tetherPoint.localPosition += (Vector3)delta;

        tetherPoint.localPosition = tetherPoint.localPosition.normalized * tetherLength;
        tetherPoint.localEulerAngles = new Vector3(0, 0, CalculateTetherAngle());
    }

    private void UpdateIconNotifs()
    {
        var currentIcon = GetIconClosestToPointer();

        if (currentIcon != lastIcon)
        {
            if (lastIcon && lastIcon.GetHovering())
            {
                lastIcon.OnTetherExit();
            }

            if (currentIcon && !currentIcon.GetHovering())
            {
                currentIcon.OnTetherEnter();
            }

            FMODUWE.PlayOneShot(hoverSFX, transform.position);
        }

        if (!currentIcon)
        {
            selectionHighlight.gameObject.SetActive(false);
        }
        else if (currentIcon.GetHovering())
        {
            selectionHighlight.gameObject.SetActive(true);
            float increment = distributor.GetIncrement();
            selectionHighlight.transform.localEulerAngles = new Vector3(0, 0, currentIcon.transform.GetSiblingIndex() * increment - increment / 2);
        }

        lastIcon = currentIcon;
    }

    private void UpdateSelection()
    {
        if (!GameInput.GetButtonDown(GameInput.Button.LeftHand)) return;

        if (!lastIcon) return;

        if (selectedIcon) selectedIcon.Deselect();

        SelectIcon(lastIcon);
    }

    public void SelectIcon(RadialIcon icon, bool forceColSwap = false, bool playSFX = true)
    {
        icon.Select();
        selectedIcon = icon;
        selectionPreview.sprite = icon.GetAbility().GetSprite();
        onAbilitySelected?.Invoke();

        if (forceColSwap)
        {
            icon.ForceColorSwap();
        }

        if (playSFX) FMODUWE.PlayOneShot(selectSFX, transform.position);
    }

    private void HandleActivation()
    {
        if (menuOpen) return;

        if (Player.main.currChair != chair) return;

        if (!GameInput.GetButtonDown(GameInput.Button.RightHand)) return;

        if (!selectedIcon) return;

        selectedIcon.Activate();
    }

    private RadialIcon GetIconClosestToPointer()
    {
        return distributor.GetIconClosestToAngle(CalculateTetherAngle()).GetComponent<RadialIcon>();
    }

    public RadialIcon GetSelectedIcon()
    {
        return selectedIcon;
    }

    private float CalculateTetherAngle()
    {
        if (timeLastAngleCalculated == Time.time)
        {
            return lastTetherAngle;
        }

        float angle = Mathf.Atan2(tetherPoint.localPosition.y, tetherPoint.localPosition.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360;

        lastTetherAngle = angle;
        timeLastAngleCalculated = Time.time;

        return angle;
    }

    public void SetMenuOpen(bool open)
    {
        menuOpen = open;
        selectionHighlight.gameObject.SetActive(open);
        if (open)
        {
            timeOpened = Time.time;
            FMODUWE.PlayOneShot(openSFX, transform.position);
        }
    }

    public void RegenerateHighlightArc()
    {
        float increment = distributor.GetIncrement();
        selectionHighlight.SetTargetAngle(increment);
    }

    public PilotingChair GetPilotingChair() => chair;
}
