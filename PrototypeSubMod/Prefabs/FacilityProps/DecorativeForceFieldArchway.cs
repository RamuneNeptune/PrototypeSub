using Nautilus.Assets.PrefabTemplates;
using Nautilus.Assets;
using UnityEngine;

namespace PrototypeSubMod.Prefabs.FacilityProps;

internal class DecorativeForceFieldArchway : MonoBehaviour
{
    public static PrefabInfo prefabInfo { get; private set; }

    public static void Register()
    {
        prefabInfo = PrefabInfo.WithTechType("DecorativeForceFieldArchway", null, null, "English");

        var prefab = new CustomPrefab(prefabInfo);
        var cloneTemplate = new CloneTemplate(prefabInfo, "c87c75bd-0342-4ece-8f9b-a1a22c205456");
        cloneTemplate.ModifyPrefab += gameObject =>
        {
            var motorChanges = gameObject.transform.Find("motormodechanges").gameObject;
            var volumeTriggers = gameObject.transform.Find("watervolumetriggers").gameObject;

            GameObject.Destroy(motorChanges);
            GameObject.Destroy(volumeTriggers);
        };

        prefab.SetGameObject(cloneTemplate);

        prefab.Register();
    }
}
