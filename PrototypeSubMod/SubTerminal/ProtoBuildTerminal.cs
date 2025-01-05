using PrototypeSubMod.Prefabs;
using Story;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PrototypeSubMod.SubTerminal;

internal class ProtoBuildTerminal : Crafter
{
    [SerializeField] private float buildDuration = 20f;
    [SerializeField] private float buildDelay;
    [SerializeField] private FMODAsset buildSoundEffect;
    [SerializeField] private Transform sfxSpawnPos;
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

    private IEnumerator StartCraftChargeUp(TechType techType, float duration)
    {
        screenManager.BeginBuildStage();
        spikesAnimator.SetTrigger("BuildWarmup");
        animScreen.StartPreWarm(buildDelay);

        foreach (var item in batteryManagers)
        {
            item.StartBatteryCharge(buildDelay);
        }

        yield return new WaitForSeconds(buildDelay);

        base.Craft(techType, duration);
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

        var prefab = CraftData.GetPrefabForTechTypeAsync(techType);
        yield return prefab;

        FMODUWE.PlayOneShot(buildSoundEffect, sfxSpawnPos.position);
        var instantiatedPrefab = Instantiate(prefab.result.Get(), buildPosition.position, buildPosition.rotation);
        instantiatedPrefab.SetActive(true);
        prefab = null;

        CrafterLogic.NotifyCraftEnd(instantiatedPrefab, techType);
        ItemGoalTracker.OnConstruct(techType);
        VFXConstructing vfxConstructing = instantiatedPrefab.GetComponentInChildren<VFXConstructing>();
        if (vfxConstructing != null)
        {
            vfxConstructing.timeToConstruct = duration;
            vfxConstructing.StartConstruction();
        }

        vfxConstructing.informGameObject = gameObject;

        animScreen.StartAnimation(duration + vfxConstructing.delay);

        LargeWorldEntity.Register(instantiatedPrefab);
        SendBuildBots(instantiatedPrefab);
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
