using System;
using PrototypeSubMod.Prefabs;
using Story;
using System.Collections;
using Nautilus.Utility;
using PrototypeSubMod.LightDistortionField;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PrototypeSubMod.SubTerminal;

internal class ProtoBuildTerminal : Crafter
{
    [SerializeField] private float buildDuration = 20f;
    [SerializeField] private float buildDelay;
    [SerializeField] private FMODAsset buildSoundEffect;
    [SerializeField] private FMOD_CustomEmitter chargeUpSFX;
    [SerializeField] private FMOD_CustomEmitter dischargeSFX;
    [SerializeField] private Transform buildPosition;
    [SerializeField] private GameObject upgradeIconPrefab;
    [SerializeField] private ProtoBatteryManager[] batteryManagers;
    [SerializeField] private ProtoBuildBot[] buildBots;
    [SerializeField] private Animator spikesAnimator;
    [SerializeField] private WarpInFXPlayer warpFXSpawner;
    [SerializeField] private SubReconstructionManager reconstructionManager;

    [Header("Screens")]
    [SerializeField] private BuildTerminalScreenManager screenManager;
    [SerializeField] private uGUI_BuildAnimScreen animScreen;

    private int returnedBotCount;
    
    public void CraftSub()
    {
        Craft(Prototype_Craftable.SubInfo.TechType, buildDuration);
    }

    public override void Craft(TechType techType, float duration)
    {
        if (!CrafterLogic.ConsumeResources(techType)) return;

        UWE.CoroutineHost.StartCoroutine(StartCraftChargeUp(duration));
        UWE.CoroutineHost.StartCoroutine(StartReconstruction(reconstructionManager));
        StoryGoalManager.main.OnGoalComplete("PrototypeCrafted");
    }

    public void RebuildSub()
    {
        UWE.CoroutineHost.StartCoroutine(StartCraftChargeUp(buildDuration));
        UWE.CoroutineHost.StartCoroutine(StartReconstruction(reconstructionManager));
    }
    
    public void RecentralizeSub()
    {
        UWE.CoroutineHost.StartCoroutine(StartCraftChargeUp(buildDuration));
        UWE.CoroutineHost.StartCoroutine(RecentralizeSubDelayed());
    }

    private IEnumerator StartCraftChargeUp(float duration)
    {
        chargeUpSFX.Play();
        StartCoroutine(PlayDischargeDelayed());
        screenManager.BeginBuildStage();
        spikesAnimator.SetTrigger("BuildWarmup");
        animScreen.StartPreWarm(buildDelay);

        foreach (var item in batteryManagers)
        {
            item.StartBatteryCharge(buildDelay);
        }

        yield return new WaitForSeconds(buildDelay);
        
        foreach (var item in batteryManagers)
        {
            item.StartBatteryDrain(duration);
        }
    }
    
    private void StartConstruction(GameObject instantiatedPrefab, TechType techType, float duration)
    {
        screenManager.OnConstructionStarted();
        FMODUWE.PlayOneShot(buildSoundEffect, buildPosition.position);

        CrafterLogic.NotifyCraftEnd(instantiatedPrefab, techType);
        ItemGoalTracker.OnConstruct(techType);
        var vfxConstructing = instantiatedPrefab.GetComponent<VFXConstructing>();
        if (!vfxConstructing) throw new Exception($"No VFXConstructing component on {instantiatedPrefab}");
            
        vfxConstructing.enabled = true;
        vfxConstructing.timeToConstruct = duration;
        vfxConstructing.StartConstruction();
        vfxConstructing.informGameObject = gameObject;

        animScreen.StartAnimation(duration + vfxConstructing.delay);

        LargeWorldEntity.Register(instantiatedPrefab);
        SendBuildBots(instantiatedPrefab);
        Plugin.GlobalSaveData.prototypePresent = true;
    }

    private IEnumerator StartReconstruction(SubReconstructionManager manager)
    {
        yield return new WaitForSeconds(buildDelay);

        manager.OnConstructionStarted(buildPosition.position, buildPosition.rotation);
        var sub = manager.GetSubObject();
        sub.transform.position = buildPosition.position;
        sub.transform.rotation = buildPosition.rotation;
        sub.gameObject.SetActive(true);
        warpFXSpawner.SpawnWarpInFX(buildPosition.position, Vector3.one * 2f);
        
        yield return new WaitForEndOfFrame();
        var constructing = sub.GetComponent<VFXConstructing>();
        constructing.ghostMaterial = MaterialUtils.GhostMaterial;
        constructing.delay = 2;
        yield return new WaitForEndOfFrame();
        
        StartConstruction(sub, TechType.None, buildDuration);

        // Failsafe end construct to fix Octo's weird bug
        yield return new WaitForSeconds(buildDuration + 0.2f);
        if (!constructing.isDone && !constructing.enabled)
        {
            constructing.enabled = true;
        }
        
        Plugin.Logger.LogInfo("Sub should be finished building");
        ErrorMessage.AddDebug("Sub should be finished building");
    }

    private IEnumerator RecentralizeSubDelayed()
    {
        yield return new WaitForSeconds(buildDelay);

        if (CloakEffectHandler.EffectHandlers.Count == 0) throw new Exception("No subs in scene to recentralize");

        var root = CloakEffectHandler.EffectHandlers[0].GetComponentInParent<SubRoot>();
        root.transform.position = buildPosition.position;
        root.transform.rotation = buildPosition.rotation;
        warpFXSpawner.SpawnWarpInFX(buildPosition.position, Vector3.one * 2f);
        screenManager.EndBuildStage();
    }

    private IEnumerator PlayDischargeDelayed()
    {
        yield return new WaitForSeconds(10.6f);
        dischargeSFX.Play();
    }

    private void SendBuildBots(GameObject toBuild)
    {
        returnedBotCount = 0;

        var botPaths = toBuild.GetComponentsInChildren<BuildBotPath>();
        if (botPaths.Length == 0)
        {
            Plugin.Logger.LogError($"No bot paths found on {toBuild}");
            return;
        }

        spikesAnimator.enabled = false;

        for (int i = 0; i < buildBots.Length; i++)
        {
            int index = i % botPaths.Length;
            buildBots[i].SetPath(botPaths[index], toBuild);
        }
    }

    // Called by VFX Constructing
    public void OnConstructionDone(GameObject constructedObject)
    {
        for (int i = 0; i < buildBots.Length; i++)
        {
            buildBots[i].FinishConstruction(OnBotReturned);
        }

        screenManager.EndBuildStage();
    }

    private void OnBotReturned()
    {
        returnedBotCount++;
        if (returnedBotCount >= buildBots.Length)
        {
            spikesAnimator.enabled = true;
            foreach (var item in buildBots)
            {
                item.OnAllBotsReturned();
            }
        }
    }
}
