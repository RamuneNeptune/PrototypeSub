using Nautilus.Assets;
using Nautilus.Assets.PrefabTemplates;
using UnityEngine;

namespace PrototypeSubMod.Prefabs;

internal class InterceptorIslandTeleporterKey_World
{
    public static PrefabInfo prefabInfo { get; private set; }

    public static void Register()
    {
        prefabInfo = PrefabInfo.WithTechType("ProtoIslandTeleporterKey", null, null, "English")
            .WithSizeInInventory(new Vector2int(2, 2))
            .WithIcon(Plugin.AssetBundle.LoadAsset<Sprite>("AlienFramework_Icon"));

        var prefab = new CustomPrefab(prefabInfo);
        var cloneTemplate = new CloneTemplate(prefabInfo, TechType.PrecursorKey_Purple);
        cloneTemplate.ModifyPrefab += gameObject =>
        {
            Texture2D replacementGlyph = Plugin.AssetBundle.LoadAsset<Texture2D>("InterceptorGlyph");
            var rend1 = gameObject.transform.Find("Model/Rig_J/precursor_key_C_02_symbol_05").GetComponent<Renderer>();
            var rend2 = gameObject.transform.Find("/ViewModel/Rig_J/precursor_key_C_02_symbol_05").GetComponent<Renderer>();

            var tempMats = rend1.materials;
            tempMats[1].mainTexture = replacementGlyph;
            rend1.materials = tempMats;

            tempMats = rend2.materials;
            tempMats[1].mainTexture = replacementGlyph;
            rend2.materials = tempMats;
        };

        prefab.SetGameObject(cloneTemplate);

        prefab.Register();
    }
}
