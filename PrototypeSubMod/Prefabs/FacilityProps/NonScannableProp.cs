using Nautilus.Assets.PrefabTemplates;
using Nautilus.Assets;

namespace PrototypeSubMod.Prefabs.FacilityProps;

internal class NonScannableProp
{
    public static void Register(string classID, string newTechType)
    {
        var prefabInfo = PrefabInfo.WithTechType(newTechType, null, null, "English");

        var prefab = new CustomPrefab(prefabInfo);
        var cloneTemplate = new CloneTemplate(prefabInfo, classID);
        cloneTemplate.ModifyPrefab += gameObject =>
        {
            var tag = gameObject.GetComponent<TechTag>();
            tag.type = prefabInfo.TechType;
        };

        prefab.SetGameObject(cloneTemplate);

        prefab.Register();
    }
}
