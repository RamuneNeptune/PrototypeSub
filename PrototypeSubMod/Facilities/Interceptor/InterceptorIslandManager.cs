using System;
using PrototypeSubMod.Patches;
using PrototypeSubMod.Utility;
using System.Collections;
using PrototypeSubMod.Prefabs;
using UnityEngine;

namespace PrototypeSubMod.Facilities.Interceptor;

internal class InterceptorIslandManager : MonoBehaviour
{
    [SaveStateReference]
    public static InterceptorIslandManager Instance;

    [SerializeField] private ProtoUpgradeCategory interceptorCategory;
    [SerializeField] private PrecursorTeleporter teleporter;
    [SerializeField] private GameObject islandObjects;
    [SerializeField] private DummyTechType emergencyWarpTechType;
    [SerializeField] private Transform respawnPoint;

    private Vector3 voidTeleportPos;
    private Vector3 originalTeleportPos;

    private void Awake()
    {
        if (Instance != null) throw new System.Exception("More than one InterceptorIslandManager in the scene. How did this even happen?");

        Instance = this;
    }

    private void Start()
    {
        originalTeleportPos = teleporter.warpToPos;
        UnlockProtoUpgrade.OnCategoryUnlocked += OnCategoryComplete;
        SetIslandEnabled(false);
    }

    private void OnCategoryComplete(ProtoUpgradeCategory category)
    {
        if (interceptorCategory != category) return;
        
        KnownTech.Add(InterceptorFacilityKey.prefabInfo.TechType);
        PDAEncyclopedia.Add("InterceptorFacilityTabletEncy", true);
    }

    public void SetIslandEnabled(bool enabled)
    {
        islandObjects.SetActive(enabled);
        if (enabled) UpdateTeleportPos();
    }

    public void OnTeleportToIsland(Vector3 voidPosition)
    {
        voidTeleportPos = voidPosition;
        SetIslandEnabled(true);
    }

    // Called via PrecursorTeleporterCollider
    public void BeginTeleportPlayer()
    {
        if (!teleporter.isOpen) return;
        
        StartCoroutine(OnTeleportPlayer());
    }

    public void UpdateTeleportPos()
    {
        if (!Plugin.GlobalSaveData.reactorSequenceComplete)
        {
            teleporter.warpToPos = voidTeleportPos;
        }
        else
        {
            teleporter.warpToPos = originalTeleportPos;
        }
    }

    private IEnumerator OnTeleportPlayer()
    {
        yield return new WaitForSeconds(2f);

        if (InterceptorReactorSequenceManager.SequenceInProgress)
        {
            InterceptorReactorSequenceManager.OnTeleportToVoid();
            GUIController.SetHidePhase(GUIController.HidePhase.HUD);
            GUIController_Patches.SetDenyHideCycling(true);
            UWE.CoroutineHost.StartCoroutine(PlayVoicelineDelayed());

            yield return new WaitForSeconds(1f);
        }

        SetIslandEnabled(false);
    }

    private IEnumerator PlayVoicelineDelayed()
    {
        yield return new WaitUntil(() => LargeWorldStreamer.main.IsWorldSettled());
        yield return new WaitForSeconds(1f);
        
        PDALog.Add("OnTeleportToInterceptorIsland");
    }

    public Vector3 GetRespawnPoint() => respawnPoint.position;
    public bool GetIslandEnabled() => islandObjects.activeSelf;

    public void UpdateSeaglideLights(bool forceRendererd)
    {
        var sealigdes = Inventory.main.container.GetItems(TechType.Seaglide);
        if (sealigdes == null) return;

        foreach (var item in sealigdes)
        {
            var lights = item.item.transform.Find("lights_parent").GetComponentsInChildren<Light>(true);
            foreach (var light in lights)
            {
                light.renderMode = forceRendererd ? LightRenderMode.ForcePixel : LightRenderMode.Auto;
            }
        }
    }

    private void OnDestroy()
    {
        UnlockProtoUpgrade.OnCategoryUnlocked -= OnCategoryComplete;
    }
}
