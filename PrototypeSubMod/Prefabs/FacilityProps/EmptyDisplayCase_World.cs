using Nautilus.Assets.PrefabTemplates;
using Nautilus.Assets;
using UnityEngine;

namespace PrototypeSubMod.Prefabs.FacilityProps;

internal class EmptyDisplayCase_World
{
    public static PrefabInfo prefabInfo { get; private set; }

    public static void Register()
    {
        prefabInfo = PrefabInfo.WithTechType("EmptyDisplayCase", null, null, "English");

        var prefab = new CustomPrefab(prefabInfo);
        var cloneTemplate = new CloneTemplate(prefabInfo, "11e731e7-bc82-4f94-90be-5db7b58b449b");
        cloneTemplate.ModifyPrefab += gameObject =>
        {
            var tag = gameObject.GetComponent<TechTag>();
            tag.type = prefabInfo.TechType;
        };

        prefab.SetGameObject(cloneTemplate);

        prefab.Register();
    }
}
