using Nautilus.Assets;
using Nautilus.Assets.PrefabTemplates;
using UnityEngine;

namespace PrototypeSubMod.Prefabs.FacilityProps;

internal class DeactivatedTeleporter_World
{
    public static PrefabInfo prefabInfo { get; private set; }

    public static void Register()
    {
        prefabInfo = PrefabInfo.WithTechType("ProtoDeactivatedTeleporter", null, null, "English");

        var prefab = new CustomPrefab(prefabInfo);
        var cloneTemplate = new CloneTemplate(prefabInfo, TechType.PrecursorTeleporter);
        cloneTemplate.ModifyPrefab += gameObject =>
        {
            var teleporter = gameObject.GetComponent<PrecursorTeleporter>();
            GameObject.Destroy(teleporter);
        };

        prefab.SetGameObject(cloneTemplate);

        prefab.Register();
    }
}
