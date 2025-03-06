using Nautilus.Assets;
using Nautilus.Assets.PrefabTemplates;
using UnityEngine;

namespace PrototypeSubMod.Prefabs.FacilityProps;
 
internal class KinematicPrefabClone
{
    public static void Register(string classID, string newTechType)
    {
        var prefabInfo = PrefabInfo.WithTechType(newTechType, null, null, "English");

        var prefab = new CustomPrefab(prefabInfo);
        var cloneTemplate = new CloneTemplate(prefabInfo, classID);
        cloneTemplate.ModifyPrefab += gameObject =>
        {
            var rb = gameObject.GetComponent<Rigidbody>();
            if (rb)
            {
                rb.isKinematic = true;
            }

            var pickupable = gameObject.GetComponent<Pickupable>();
            if (pickupable)
            {
                pickupable.attached = true;
            }
        };

        prefab.SetGameObject(cloneTemplate);

        prefab.Register();
    }
}
