using Nautilus.Assets;
using Nautilus.Assets.PrefabTemplates;
using UnityEngine;

namespace PrototypeSubMod.Prefabs.FacilityProps;

internal class ProtoEngineFacilityRoom
{
    public static PrefabInfo prefabInfo { get; private set; }

    public static void Register()
    {
        prefabInfo = PrefabInfo.WithTechType("ProtoEngineRoom", null, null);

        var prefab = new CustomPrefab(prefabInfo);
        var cloneTemplate = new CloneTemplate(prefabInfo, "8df14188-4856-4e42-b8ae-bbc27bfb5e4c");
        cloneTemplate.ModifyPrefab += gameObject =>
        {
            var biomeObject = gameObject.transform.Find("Precursor_LavaBase_ThermalRoom_instances/biomeArea");
            GameObject.Destroy(biomeObject.gameObject);

            var occlusionManager = gameObject.transform.Find("CullVolumeManager");
            GameObject.Destroy(occlusionManager.gameObject);
        };

        prefab.SetGameObject(cloneTemplate);
        prefab.Register();
    }
}
