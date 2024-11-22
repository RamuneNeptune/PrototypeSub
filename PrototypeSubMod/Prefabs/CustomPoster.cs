using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using UnityEngine;

namespace PrototypeSubMod.Prefabs;

internal class CustomPoster
{
    public readonly PrefabInfo prefabInfo;
    private readonly Texture2D _posterImage;

    public CustomPoster(string classId, string displayName, string description, Texture2D posterImage, Texture2D posterIcon)
    {
        prefabInfo = PrefabInfo.WithTechType(classId, displayName, description, unlockAtStart: true).WithIcon(new Atlas.Sprite(posterIcon));
        _posterImage = posterImage;

        Plugin.Logger.LogInfo($"Registering poster with classID = {classId}");
        var prefab = new CustomPrefab(prefabInfo);
        var cloneTemplate = new CloneTemplate(prefabInfo, TechType.PosterAurora);
        cloneTemplate.ModifyPrefab = prefab =>
        {
            var material = prefab.GetComponentInChildren<MeshRenderer>().materials[1];
            material.mainTexture = _posterImage;
            material.SetTexture(ShaderPropertyID._SpecTex, _posterImage);
        };

        prefab.SetGameObject(cloneTemplate);

        prefab.SetEquipment(EquipmentType.Hand).WithQuickSlotType(QuickSlotType.Selectable);

        prefab.Register();
    }
}
