using PrototypeSubMod.Patches;
using PrototypeSubMod.UI.ProceduralArcGenerator;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PrototypeSubMod.UI.AbilitySelection;

public class TetherManager : MonoBehaviour
{
    [SerializeField] private Transform tetherPoint;
    [SerializeField] private CircularMeshApplier selectionHighlight;
    [SerializeField] private IconDistributor distributor;
    [SerializeField] private Image selectionPreview;
    [SerializeField] private float tetherSensitivity;
    [SerializeField] private float tetherLength;
    [SerializeField] private float cursorAngleMultiplier;

    private RadialIcon lastIcon;
    private RadialIcon selectedIcon;

    private float lastTetherAngle;
    private float timeLastAngleCalculated;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        selectionHighlight.gameObject.SetActive(false);
        selectionPreview.gameObject.SetActive(false);

        float increment = distributor.GetIncrement();
        selectionHighlight.SetTargetAngle(increment);
    }

    private void Update()
    {
        UpdateTetherPoint();
        UpdateIconNotifs();
        UpdateSelection();
        HandleActivation();
    }

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
        if (!Input.GetMouseButtonDown(0)) return;

        if (!lastIcon) return;

        if (selectedIcon) selectedIcon.Deselect();

        lastIcon.Select();
        selectedIcon = lastIcon;
        selectionPreview.gameObject.SetActive(true);
        selectionPreview.sprite = selectedIcon.GetAbility().GetSprite();
    }

    private void HandleActivation()
    {
        if (!Input.GetMouseButtonDown(1)) return;

        if (!selectedIcon) return;

        selectedIcon.Activate();
    }

    private RadialIcon GetIconClosestToPointer()
    {
        return distributor.GetIconClosestToAngle(CalculateTetherAngle()).GetComponent<RadialIcon>();
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

    private List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = tetherPoint.transform.position
        };
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }
}
