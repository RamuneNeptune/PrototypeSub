using Nautilus.Assets.PrefabTemplates;
using Nautilus.Assets;

namespace PrototypeSubMod.Prefabs.FacilityProps;

internal class PrecursorScanner2_NoScan
{
    public static PrefabInfo prefabInfo { get; private set; }

    public static void Register()
    {
        prefabInfo = PrefabInfo.WithTechType("PrecursorScannerArmNoScan2", null, null, "English");

        var prefab = new CustomPrefab(prefabInfo);
        var cloneTemplate = new CloneTemplate(prefabInfo, "ebc943e4-200c-4789-92f3-e675cd982dbe");
        cloneTemplate.ModifyPrefab += gameObject =>
        {
            var tag = gameObject.GetComponent<TechTag>();
            tag.type = prefabInfo.TechType;
        };

        prefab.SetGameObject(cloneTemplate);

        prefab.Register();
    }
}
