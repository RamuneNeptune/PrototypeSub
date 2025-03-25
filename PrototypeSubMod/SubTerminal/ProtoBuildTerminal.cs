using PrototypeSubMod.Prefabs;
using Story;
using System.Collections;
using Nautilus.Utility;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PrototypeSubMod.SubTerminal;

internal class ProtoBuildTerminal : Crafter
{
    [SerializeField] private float buildDuration = 20f;
    [SerializeField] private float buildDelay;
    [SerializeField] private FMODAsset buildSoundEffect;
    [SerializeField] private FMOD_CustomEmitter chargeUpSFX;
    [SerializeField] private Transform buildPosition;
    [SerializeField] private GameObject upgradeIconPrefab;
    [SerializeField] private ProtoBatteryManager[] batteryManagers;
    [SerializeField] private ProtoBuildBot[] buildBots;
    [SerializeField] private Animator spikesAnimator;

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

        UWE.CoroutineHost.StartCoroutine(StartCraftChargeUp(techType, duration));
    }

    public void RebuildSub(SubReconstructionManager manager)
    {
        if (!CrafterLogic.ConsumeResources(manager.GetReconstructionTechType())) return;

        UWE.CoroutineHost.StartCoroutine(StartCraftChargeUp(TechType.None, buildDuration, false));
        UWE.CoroutineHost.StartCoroutine(StartReconstruction(manager));
    }

    private IEnumerator StartCraftChargeUp(TechType techType, float duration, bool craft = true)
    {
        chargeUpSFX.Play();
        screenManager.BeginBuildStage();
        spikesAnimator.SetTrigger("BuildWarmup");
        animScreen.StartPreWarm(buildDelay);

        foreach (var item in batteryManagers)
        {
            item.StartBatteryCharge(buildDelay);
        }

        yield return new WaitForSeconds(buildDelay);

        if (craft)
        {
            base.Craft(techType, duration);
        }
        foreach (var item in batteryManagers)
        {
            item.StartBatteryDrain(buildDuration);
        }
    }

    public override void OnCraftingBegin(TechType techType, float duration)
    {
        StartCoroutine(OnCraftingBeginAsync(techType, duration));
    }

    private IEnumerator OnCraftingBeginAsync(TechType techType, float duration)
    {
        screenManager.OnConstructionStarted();
        var op = SceneManager.LoadSceneAsync("prototypesub", LoadSceneMode.Additive);
        yield return op;

        var prefab = GameObject.Find("PrototypeSub-MainPrefab");
        prefab.transform.position = buildPosition.position;
        prefab.transform.rotation = buildPosition.rotation;
        prefab.name = "PrototypeSub(Clone)";
        
        Prototype_Craftable.SetupProtoGameObject(prefab);
        
        yield return new WaitForEndOfFrame();
        prefab.GetComponent<VFXConstructing>().ghostMaterial = MaterialUtils.GhostMaterial;
        yield return new WaitForEndOfFrame();
        
        StartConstruction(prefab, techType, duration);
    }

    private void StartConstruction(GameObject instantiatedPrefab, TechType techType, float duration)
    {
        FMODUWE.PlayOneShot(buildSoundEffect, buildPosition.position);

        CrafterLogic.NotifyCraftEnd(instantiatedPrefab, techType);
        ItemGoalTracker.OnConstruct(techType);
        var vfxConstructing = instantiatedPrefab.GetComponent<VFXConstructing>();
        if (vfxConstructing != null)
        {
            vfxConstructing.enabled = true;
            vfxConstructing.timeToConstruct = duration;
            vfxConstructing.StartConstruction();
        }

        vfxConstructing.informGameObject = gameObject;

        animScreen.StartAnimation(duration + vfxConstructing.delay);

        LargeWorldEntity.Register(instantiatedPrefab);
        SendBuildBots(instantiatedPrefab);
    }

    private IEnumerator StartReconstruction(SubReconstructionManager manager)
    {
        yield return new WaitForSeconds(buildDelay);

        manager.OnConstructionStarted(buildPosition.position, buildPosition.rotation);
        var sub = manager.GetSubObject();
        sub.gameObject.SetActive(true);
        
        yield return new WaitForEndOfFrame();
        var constructing = sub.GetComponent<VFXConstructing>();
        constructing.ghostMaterial = MaterialUtils.GhostMaterial;
        constructing.delay = 2;
        yield return new WaitForEndOfFrame();
        
        StartConstruction(sub, manager.GetReconstructionTechType(), buildDuration);
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
