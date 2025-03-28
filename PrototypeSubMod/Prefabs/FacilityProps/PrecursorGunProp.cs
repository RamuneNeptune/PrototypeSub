using System;
using System.Collections.Generic;
using System.Linq;
using Nautilus.Assets;
using Nautilus.Assets.PrefabTemplates;
using UnityEngine;

namespace PrototypeSubMod.Prefabs.FacilityProps;

public class PrecursorGunProp
{
    public static void Register()
    {
        var prefabInfo = PrefabInfo.WithTechType("PrecursorGunProp", null, null, "English");

        var prefab = new CustomPrefab(prefabInfo);
        var cloneTemplate = new CloneTemplate(prefabInfo, "22fb9ee9-690d-426c-844f-a80e527b5fe6");
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

            foreach (var rend in gameObject.GetComponentsInChildren<Renderer>(true))
            {
                var materials = rend.materials;
                foreach (var mat in materials)
                {
                    mat.SetFloat("_SpecInt", 1);
                    mat.SetFloat("_Shininess", 6.02f);
                    mat.SetColor("_SpecColor", new Color(0.401f, 0.976f, 0.667f));
                }
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