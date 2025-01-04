using Nautilus.Assets.PrefabTemplates;
using Nautilus.Assets;

namespace PrototypeSubMod.Prefabs.FacilityProps;

internal class PrecursorScanner3_NoScan
{
    public static PrefabInfo prefabInfo { get; private set; }

    public static void Register()
    {
        prefabInfo = PrefabInfo.WithTechType("PrecursorScannerArmNoScan3", null, null, "English");

        var prefab = new CustomPrefab(prefabInfo);
        var cloneTemplate = new CloneTemplate(prefabInfo, "ac2b0798-e311-4cb1-9074-fae59cd7347a");
        cloneTemplate.ModifyPrefab += gameObject =>
        {
            var tag = gameObject.GetComponent<TechTag>();
            tag.type = prefabInfo.TechType;
        };

        prefab.SetGameObject(cloneTemplate);

        prefab.Register();
    }
}
