using PrototypeSubMod.MiscMonobehaviors.SubSystems;
using PrototypeSubMod.PowerSystem.Funcionalities;
using System;
using System.Collections;
using UnityEngine;

namespace PrototypeSubMod.Teleporter;

internal class TeleporterOverride : MonoBehaviour
{
    private static readonly Color OverrideColor = new(0f, 0.285f, 0.555f, 0.588f);
    private static Color CurrentOverrideColor = OverrideColor;

    public static string FullOverrideTeleporterID { get; private set; }
    public static float OverrideTime { get; private set; }
    public static bool QueuedResetOverrideTime { get; private set; }
    public static bool QueuedTeleportedBackToSub { get; private set; }
    private static float TimeWhenPortalUnloaded;
    private static float TimeLeftWhenUnloaded;
    private static bool OverrideRequested;
    private static ProtoTeleporterManager LastOverrideOwner;

    private static event Action OnTeleportStart;

    private Vector3 originalTeleportPosition;
    private PrecursorTeleporter teleporter;
    private float originalTeleportAngle;
    private float currentOverrideTime;
    private float overrideTimeLastFrame;
    private string teleporterID;
    private bool overrideActive;

    private Material fxMaterial;
    private Color originalColor;
    private Color targetColor;

    public static void SetOverrideTeleporterID(string id)
    {
        FullOverrideTeleporterID = id;
    }

    public static void SetOverrideTime(float time)
    {
        OverrideTime = time;
    }

    public static void OnTeleportStarted(ProtoTeleporterManager overrideOwner)
    {
        QueuedResetOverrideTime = true;
        OverrideRequested = true;
        OnTeleportStart?.Invoke();
        LastOverrideOwner = overrideOwner;
    }

    public static void OnTeleportToSubFinished()
    {
        QueuedTeleportedBackToSub = false;
        OverrideRequested = false;
        LastOverrideOwner = null;
    }

    public static void SetTempTeleporterColor(Color color)
    {
        CurrentOverrideColor = color;
    }

    public static void ResetTeleporterColor()
    {
        CurrentOverrideColor = OverrideColor;
    }

    private void Start()
    {
        Initialize();
        UWE.CoroutineHost.StartCoroutine(WaitToPlayFirstStatus());
    }

    private void OnEnable() => OnTeleportStart += TargetTeleporterCheck;
    private void OnDisable() => OnTeleportStart -= TargetTeleporterCheck;

    private void Initialize()
    {
        //This stuff may look weird but remember that the portal is only loaded in when it's being teleported to, so this is called when it's loaded in

        teleporter = GetComponent<PrecursorTeleporter>();
        originalTeleportPosition = teleporter.warpToPos;
        originalTeleportAngle = teleporter.warpToAngle;

        teleporterID = teleporter.teleporterIdentifier + (GetComponentInParent<PrecursorTeleporterActivationTerminal>() != null ? "M" : "S");

        if (teleporterID != FullOverrideTeleporterID) return;

        float timeLeft = TimeWhenPortalUnloaded - Time.time + TimeLeftWhenUnloaded;
        if (QueuedResetOverrideTime)
        {
            currentOverrideTime = OverrideTime;
            QueuedResetOverrideTime = false;
        }
        else if (timeLeft > 0)
        {
            currentOverrideTime = timeLeft;
        }

        overrideActive = true;

        if (TeleporterManager.GetTeleporterActive(teleporterID))
        {
            TryRetrieveFxMaterial();
        }
    }

    private void TargetTeleporterCheck()
    {
        teleporterID = teleporter.teleporterIdentifier + (GetComponentInParent<PrecursorTeleporterActivationTerminal>() != null ? "M" : "S");

        if (teleporterID != FullOverrideTeleporterID) return;

        if (QueuedResetOverrideTime && !overrideActive)
        {
            Initialize();
        }
    }

    private void Update()
    {
        HandleOverrideCountdown();
        HandleOverrideColor();
    }

    private void HandleOverrideCountdown()
    {
        if (teleporterID != FullOverrideTeleporterID) return;

        if (!OverrideRequested) return;

        if (currentOverrideTime > 0)
        {
            currentOverrideTime -= Time.deltaTime;

            Transform teleportPos = LastOverrideOwner.GetTeleportPosition();
            teleporter.warpToPos = teleportPos.position;
            teleporter.warpToAngle = teleportPos.eulerAngles.y;
        }
        else if (currentOverrideTime <= 0 && overrideActive)
        {
            teleporter.warpToPos = originalTeleportPosition;
            teleporter.warpToAngle = originalTeleportAngle;
            overrideActive = false;
        }

        if (overrideTimeLastFrame > 30f && currentOverrideTime <= 30f)
        {
            LastOverrideOwner.PlayOverrideMarker2();
        }

        overrideTimeLastFrame = currentOverrideTime;
    }

    private void HandleOverrideColor()
    {
        if (fxMaterial == null)
        {
            TryRetrieveFxMaterial();
        }

        targetColor = overrideActive ? CurrentOverrideColor : originalColor;

        Color color = Color.Lerp(fxMaterial.GetColor("_ColorOuter"), targetColor, Time.deltaTime);

        fxMaterial.SetColor("_ColorOuter", color);
    }

    private void TryRetrieveFxMaterial()
    {
        GameObject fxPrefab = teleporter.portalFxSpawnPoint.Find("x_PrecursorTeleporter_LargePortal(Clone)/x_Portal").gameObject;

        if (!fxPrefab) return;

        fxMaterial = fxPrefab.GetComponent<MeshRenderer>().material;
        originalColor = fxMaterial.GetColor("_ColorOuter");
    }

    public void BeginTeleportPlayer(GameObject _)
    {
        if (overrideActive)
        {
            QueuedTeleportedBackToSub = true;
            Player.main.SetPrecursorOutOfWater(false);

            if (CurrentOverrideColor != OverrideColor)
            {
                var teleportManager = Camera.main.GetComponent<ProtoScreenTeleporterFXManager>();
                teleportManager.SetColors(ElectricubePowerFunctionality.TeleportScreenColInner, ElectricubePowerFunctionality.TeleportScreenColMiddle, ElectricubePowerFunctionality.TeleportScreenColOuter);
            }
        }
    }

    // Called in PrecursorTeleporterActivationTerminal via BroadcastMessage
    public void ToggleDoor(bool _)
    {
        TryRetrieveFxMaterial();
    }

    private void OnDestroy()
    {
        if (overrideActive)
        {
            TimeWhenPortalUnloaded = Time.time;
            TimeLeftWhenUnloaded = currentOverrideTime;
        }
    }

    private IEnumerator WaitToPlayFirstStatus()
    {
        yield return new WaitUntil(LargeWorldStreamer.main.IsWorldSettled);

        if (LastOverrideOwner != null)
        {
            LastOverrideOwner.PlayOverrideMarker1();
        }
    }
}
