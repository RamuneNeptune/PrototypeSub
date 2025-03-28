using System;
using System.Collections.Generic;
using System.Linq;
using Nautilus.Assets;
using Nautilus.Assets.PrefabTemplates;
using UnityEngine;

namespace PrototypeSubMod.Prefabs.FacilityProps;

public class PrecursorCube1Prop
{
    public static void Register()
    {
        var prefabInfo = PrefabInfo.WithTechType("PrecursorCube1Prop", null, null, "English");

        var prefab = new CustomPrefab(prefabInfo);
        var cloneTemplate = new CloneTemplate(prefabInfo, "6b0104e8-979e-46e5-bc17-57c4ac2e6e39");
        cloneTemplate.ModifyPrefab += gameObject =>
        {
            var lwe = gameObject.GetComponent<LargeWorldEntity>();
            lwe.cellLevel = LargeWorldEntity.CellLevel.VeryFar;
            
            foreach (var component in gameObject.GetComponentsInChildren<Component>(true))
            {
                var type = component.GetType();
                if (whitelistedComponents.Any(t => type.IsSubclassOf(t) || type == t)) continue;
                
                GameObject.Destroy(component);
            }
        };

        prefab.SetGameObject(cloneTemplate);

        prefab.Register();
    }
    
    private static readonly List<Type> whitelistedComponents = new()
    {
        typeof(Transform),
        typeof(Renderer),
        typeof(MeshFilter),
        typeof(PrefabIdentifier),
        typeof(LargeWorldEntity),
        typeof(SkyApplier),
        typeof(Collider),
        typeof(LODGroup)
    };
}