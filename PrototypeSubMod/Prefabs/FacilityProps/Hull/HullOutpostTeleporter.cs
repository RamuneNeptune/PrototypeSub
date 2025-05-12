using Nautilus.Assets;
using Nautilus.Assets.PrefabTemplates;
using UnityEngine;

namespace PrototypeSubMod.Prefabs.FacilityProps.Hull;

public class HullOutpostTeleporter
{
    public static PrefabInfo prefabInfo { get; private set; }

    public static void Register()
    {
        prefabInfo = PrefabInfo.WithTechType("HullOutpostTeleporter", null, null, "English");

        var prefab = new CustomPrefab(prefabInfo);
        var cloneTemplate = new CloneTemplate(prefabInfo, "9a5ff289-4b82-4b9c-b9f6-d60f46dd2d7a");
        cloneTemplate.ModifyPrefab += gameObject =>
        {
            var teleporter = gameObject.GetComponent<PrecursorTeleporter>();
            teleporter.teleporterIdentifier = "protohullfacilitytp";
            teleporter.warpToPos = new Vector3(-1071.471f, -435.393f, -1241.181f);
            teleporter.warpToAngle = 55;

            gameObject.GetComponent<TechTag>().type = TechType.PrecursorTeleporter;
        };

        prefab.SetGameObject(cloneTemplate);

        prefab.Register();
    }
}