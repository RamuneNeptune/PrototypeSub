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
    [SerializeField] private Transform buildPosition;

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

        Plugin.Logger.LogInfo($"Crafting");
        base.Craft(techType, duration);
    }

    public override void OnCraftingBegin(TechType techType, float duration)
    {
        Plugin.Logger.LogInfo($"Crafting began");
        StartCoroutine(OnCraftingBeginAsync(techType, duration));
    }

    private IEnumerator OnCraftingBeginAsync(TechType techType, float duration)
    {
        var prefab = new TaskResult<GameObject>();
        yield return CraftData.InstantiateFromPrefabAsync(techType, prefab);

        var instantiatedPrefab = prefab.Get();
        instantiatedPrefab.transform.position = buildPosition.position;
        instantiatedPrefab.transform.rotation = buildPosition.rotation;
        prefab = null;

        CrafterLogic.NotifyCraftEnd(instantiatedPrefab, techType);
        ItemGoalTracker.OnConstruct(techType);
        VFXConstructing vfxConstructing = instantiatedPrefab.GetComponentInChildren<VFXConstructing>();
        if(vfxConstructing != null)
        {
            vfxConstructing.timeToConstruct = duration;
            vfxConstructing.StartConstruction();
        }

        LargeWorldEntity.Register(instantiatedPrefab);
    }

    private void SendBuildBots(GameObject toBuild)
    {
        throw new System.NotImplementedException("Build bots not implemented yet");
    }
}
