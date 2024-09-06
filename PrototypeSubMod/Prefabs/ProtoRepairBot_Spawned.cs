using Nautilus.Assets.PrefabTemplates;
using Nautilus.Assets;
using PrototypeSubMod.Extensions;
using UnityEngine;
using PrototypeSubMod.RepairBots;
using System.Collections;

namespace PrototypeSubMod.Prefabs;

internal class ProtoRepairBot_Spawned
{
    public static PrefabInfo prefabInfo { get; private set; }

    public static void Register()
    {
        prefabInfo = PrefabInfo.WithTechType("ProtoRepairBot", null, null, "English")
            .WithIcon(SpriteManager.Get(TechType.PrecursorDroid));

        var prefab = new CustomPrefab(prefabInfo);

        prefab.SetGameObject(GetPrefab);
        prefab.Register();
    }

    private static IEnumerator GetPrefab(IOut<GameObject> prefabOut)
    {
        GameObject model = Plugin.AssetBundle.LoadAsset<GameObject>("ProtoRepairBot");

        model.SetActive(false);
        GameObject prefab = GameObject.Instantiate(model);

        var botTask = CraftData.GetPrefabForTechTypeAsync(TechType.PrecursorDroid);
        yield return botTask;

        var bot = GameObject.Instantiate(botTask.GetResult(), prefab.transform.GetChild(0));
        RemoveBotComponents(ref bot);

        prefabOut.Set(prefab);
    }

    private static void RemoveBotComponents(ref GameObject gameObject)
    {
        gameObject.RemoveComponentImmediate<CaveCrawler>();
        gameObject.RemoveComponentImmediate<Rigidbody>();
        gameObject.RemoveComponentImmediate<SphereCollider>();
        gameObject.RemoveComponentImmediate<BoxCollider>();
        gameObject.RemoveComponentImmediate<WorldForces>();
        gameObject.RemoveComponentImmediate<CaveCrawlerGravity>();
        gameObject.RemoveComponentImmediate<LiveMixin>();
        gameObject.RemoveComponentImmediate<CreatureDeath>();
        gameObject.RemoveComponentImmediate<CreatureFlinch>();
        gameObject.RemoveComponentImmediate<Locomotion>();
        gameObject.RemoveComponentImmediate<SplineFollowing>();
        gameObject.RemoveComponentImmediate<OnSurfaceMovement>();
        gameObject.RemoveComponentImmediate<WalkBehaviour>();
        gameObject.RemoveComponentImmediate<OnSurfaceTracker>();
        gameObject.RemoveComponentImmediate<RemoveSoundsOnKill>();
        gameObject.RemoveComponentImmediate<CreatureUtils>();
        gameObject.RemoveComponentImmediate<MoveOnSurface>();
        gameObject.RemoveComponentImmediate<StayAtLeashPosition>();
        gameObject.RemoveComponentImmediate<CrawlerAttackLastTarget>();
        gameObject.RemoveComponentImmediate<FleeOnDamage>();
        gameObject.RemoveComponentImmediate<CrawlerAvoidEdges>();
        gameObject.RemoveComponentImmediate<AggressiveWhenSeeTarget>();
        gameObject.RemoveComponentImmediate<MeleeAttack>();
        gameObject.RemoveComponentImmediate<LastTarget>();
        gameObject.RemoveComponentImmediate<CreatureFear>();
    }
}
