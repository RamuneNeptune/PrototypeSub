using Nautilus.Assets;
using Nautilus.Assets.PrefabTemplates;
using UnityEngine;

namespace PrototypeSubMod.Prefabs.FacilityProps;

public class NonScanWaterPipe
{
    public static PrefabInfo prefabInfo;

    public static void Register()
    {
        prefabInfo = PrefabInfo.WithTechType("NonScanWaterPipe", null, null);
        
        var prefab = new CustomPrefab(prefabInfo);
        var cloneTemplate = new CloneTemplate(prefabInfo, "fe481885-c591-49fa-aedf-0ee679f4a6b4");
        cloneTemplate.ModifyPrefab += gameObject =>
        {
            var cullManager = gameObject.transform.Find("CullVolumeManager");
            GameObject.Destroy(cullManager.gameObject);
        };

        prefab.SetGameObject(cloneTemplate);

        prefab.Register();
    }
}