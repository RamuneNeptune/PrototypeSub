using Nautilus.Assets.PrefabTemplates;
using Nautilus.Assets;

namespace PrototypeSubMod.Prefabs.FacilityProps;

internal class PrecursorScanner1_NoScan
{
    public static PrefabInfo prefabInfo { get; private set; }

    public static void Register()
    {
        prefabInfo = PrefabInfo.WithTechType("PrecursorScannerArmNoScan2", null, null, "English");

        var prefab = new CustomPrefab(prefabInfo);
        var cloneTemplate = new CloneTemplate(prefabInfo, "4f5905f8-ea50-49e8-b24f-44139c6bddcf");
        cloneTemplate.ModifyPrefab += gameObject =>
        {
            var tag = gameObject.GetComponent<TechTag>();
            tag.type = prefabInfo.TechType;
        };

        prefab.SetGameObject(cloneTemplate);

        prefab.Register();
    }
}
