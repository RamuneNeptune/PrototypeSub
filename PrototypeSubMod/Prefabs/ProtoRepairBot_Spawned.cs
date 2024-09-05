using Nautilus.Assets.PrefabTemplates;
using Nautilus.Assets;
using PrototypeSubMod.Extensions;
using UnityEngine;
using PrototypeSubMod.RepairBots;

namespace PrototypeSubMod.Prefabs;

internal class ProtoRepairBot_Spawned
{
    public static PrefabInfo prefabInfo { get; private set; }

    public static void Register()
    {
        prefabInfo = PrefabInfo.WithTechType("ProtoRepairBot", null, null, "English")
            .WithIcon(SpriteManager.Get(TechType.PrecursorDroid));

        var prefab = new CustomPrefab(prefabInfo);

        var cloneTemplate = new CloneTemplate(prefabInfo, TechType.PrecursorDroid);

        cloneTemplate.ModifyPrefab += gameObject =>
        {
            gameObject.RemoveComponent<CaveCrawler>();
            gameObject.RemoveComponent<Rigidbody>();
            gameObject.RemoveComponent<SphereCollider>();
            gameObject.RemoveComponent<BoxCollider>();
            gameObject.RemoveComponent<WorldForces>();
            gameObject.RemoveComponent<CaveCrawlerGravity>();
            gameObject.RemoveComponent<LiveMixin>();
            gameObject.RemoveComponent<CreatureDeath>();
            gameObject.RemoveComponent<CreatureFlinch>();
            gameObject.RemoveComponent<Locomotion>();
            gameObject.RemoveComponent<SplineFollowing>();
            gameObject.RemoveComponent<OnSurfaceMovement>();
            gameObject.RemoveComponent<WalkBehaviour>();
            gameObject.RemoveComponent<OnSurfaceTracker>();
            gameObject.RemoveComponent<RemoveSoundsOnKill>();
            gameObject.RemoveComponent<CreatureUtils>();
            gameObject.RemoveComponent<MoveOnSurface>();
            gameObject.RemoveComponent<StayAtLeashPosition>();
            gameObject.RemoveComponent<CrawlerAttackLastTarget>();
            gameObject.RemoveComponent<FleeOnDamage>();
            gameObject.RemoveComponent<CrawlerAvoidEdges>();
            gameObject.RemoveComponent<AggressiveWhenSeeTarget>();
            gameObject.RemoveComponent<MeleeAttack>();
            gameObject.RemoveComponent<LastTarget>();
            gameObject.RemoveComponent<CreatureFear>();

            gameObject.AddComponent<ProtoRepairBot>();
        };

        prefab.SetGameObject(cloneTemplate);
        prefab.Register();
    }
}
