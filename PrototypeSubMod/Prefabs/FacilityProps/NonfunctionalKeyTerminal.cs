using Nautilus.Assets.PrefabTemplates;
using Nautilus.Assets;
using UnityEngine;

namespace PrototypeSubMod.Prefabs.FacilityProps;

internal class NonfunctionalKeyTerminal
{
    public static PrefabInfo prefabInfo { get; private set; }

    public static void Register()
    {
        prefabInfo = PrefabInfo.WithTechType("NonfunctionalKeyTerminal", null, null, "English");

        var prefab = new CustomPrefab(prefabInfo);
        var cloneTemplate = new CloneTemplate(prefabInfo, "cbf21035-a26b-45bc-bab2-93f084e59922");
        cloneTemplate.ModifyPrefab += gameObject =>
        {
            var keyTerminal = gameObject.GetComponent<PrecursorKeyTerminal>();
            var trigger = gameObject.transform.Find("Trigger").gameObject;

            GameObject.Destroy(keyTerminal);
            GameObject.Destroy(trigger);
        };

        prefab.SetGameObject(cloneTemplate);

        prefab.Register();
    }
}
