using PrototypeSubMod.PowerSystem;
using PrototypeSubMod.Prefabs;
using Story;
using SubLibrary.SaveData;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace PrototypeSubMod.SubTerminal;

internal class ProtoBuildTerminal : Crafter
{
    public bool HasBuiltProtoSub
    {
        get
        {
            return prototypeSub != null;
        }
    }

    [SerializeField] private float buildDuration = 20f;
    [SerializeField] private FMODAsset buildSoundEffect;
    [SerializeField] private Transform buildPosition;
    [SerializeField] private GameObject upgradeIconPrefab;

    private GameObject prototypeSub;

    new private void Start()
    {
        var serializationManagers = FindObjectsOfType<SubSerializationManager>();
        prototypeSub = serializationManagers.FirstOrDefault(s => s.GetComponentInChildren<PrototypePowerSystem>() != null)?.gameObject;
    }

    public void CraftSub()
    {
        Craft(Prototype_Craftable.SubInfo.TechType, buildDuration);
    }

    public override void Craft(TechType techType, float duration)
    {
        if (!CrafterLogic.ConsumeResources(techType)) return;

        base.Craft(techType, duration);
    }

    public override void OnCraftingBegin(TechType techType, float duration)
    {
        StartCoroutine(OnCraftingBeginAsync(techType, duration));
    }

    private IEnumerator OnCraftingBeginAsync(TechType techType, float duration)
    {
        var prefab = CraftData.GetPrefabForTechTypeAsync(techType);
        yield return prefab;

        FMODUWE.PlayOneShot(buildSoundEffect, transform.position);
        var instantiatedPrefab = Instantiate(prefab.result.Get(), buildPosition.position, buildPosition.rotation);
        instantiatedPrefab.SetActive(true);
        prefab = null;

        CrafterLogic.NotifyCraftEnd(instantiatedPrefab, techType);
        ItemGoalTracker.OnConstruct(techType);
        VFXConstructing vfxConstructing = instantiatedPrefab.GetComponentInChildren<VFXConstructing>();
        if(vfxConstructing != null)
        {
            vfxConstructing.timeToConstruct = duration;
            vfxConstructing.StartConstruction();
        }

        vfxConstructing.informGameObject = gameObject;

        GetComponentInChildren<uGUI_ProtoBuildScreen>().OnConstructionStarted(duration + vfxConstructing.delay);

        LargeWorldEntity.Register(instantiatedPrefab);
    }

    private void SendBuildBots(GameObject toBuild)
    {
        throw new System.NotImplementedException("Build bots not implemented yet");
    }
}
