using UnityEngine;

namespace PrototypeSubMod.Teleporter;

internal class TeleporterOverride : MonoBehaviour
{
    private static readonly Color OverrideColor = new(0f, 0.285f, 0.555f, 0.588f);

    public static string FullOverrideTeleporterID { get; private set; }
    public static float OverrideTime { get; private set; }
    public static bool QueuedResetOverrideTime { get; private set; }
    public static bool QueuedTeleportedBackToSub { get; private set; }
    private static float TimeWhenPortalUnloaded;

    private Vector3 originalTeleportPosition;
    private PrecursorTeleporter teleporter;
    private float originalTeleportAngle;
    private float currentOverrideTime;
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

    public static void OnTeleportStarted()
    {
        QueuedResetOverrideTime = true;
    }

    public static void OnTeleportToSubFinished()
    {
        QueuedTeleportedBackToSub = false;
    }

    private void Start()
    {
        //This stuff may look weird but remember that the portal is only loaded in when it's being teleported to, so this is called when it's loaded in

        teleporter = GetComponent<PrecursorTeleporter>();
        originalTeleportPosition = teleporter.warpToPos;
        originalTeleportAngle = teleporter.warpToAngle;

        teleporterID = teleporter.teleporterIdentifier + (GetComponentInParent<PrecursorTeleporterActivationTerminal>() != null ? "M" : "S");

        if (teleporterID != FullOverrideTeleporterID) return;

        if (QueuedResetOverrideTime)
        {
            currentOverrideTime = OverrideTime;
            QueuedResetOverrideTime = false;
        }
        else if(TimeWhenPortalUnloaded - Time.time > 0)
        {
            currentOverrideTime = TimeWhenPortalUnloaded - Time.time;
        }

        overrideActive = true;
        
        if(TeleporterManager.GetTeleporterActive(teleporterID))
        {
            TryRetrieveFxMaterial();
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

        if (currentOverrideTime > 0)
        {
            currentOverrideTime -= Time.deltaTime;

            Transform teleportPos = TeleporterPositionSetter.Instance.GetTeleportPosition();
            teleporter.warpToPos = teleportPos.position;
            teleporter.warpToAngle = teleportPos.eulerAngles.y;
        }
        else if (currentOverrideTime <= 0 && overrideActive)
        {
            teleporter.warpToPos = originalTeleportPosition;
            teleporter.warpToAngle = originalTeleportAngle;
            overrideActive = false;
        }
    }

    private void HandleOverrideColor()
    {
        if (fxMaterial == null)
        {
            TryRetrieveFxMaterial();
        }

        targetColor = overrideActive ? OverrideColor : originalColor;

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
        if(overrideActive)
        {
            QueuedTeleportedBackToSub = true;
            Player.main.SetPrecursorOutOfWater(false);
        }
    }

    // Called in PrecursorTeleporterActivationTerminal via BroadcastMessage
    public void ToggleDoor(bool _)
    {
        TryRetrieveFxMaterial();
    }

    private void OnDestroy()
    {
        if(overrideActive)
        {
            TimeWhenPortalUnloaded = Time.time;
        }
    }
}
