using Nautilus.Assets;
using Nautilus.Assets.PrefabTemplates;

namespace PrototypeSubMod.Prefabs.FacilityProps;

public static class WatergateBlock
{
    public static PrefabInfo prefabInfo;
    
    public static void Register()
    {
        prefabInfo = PrefabInfo.WithTechType("WatergateBlock", null, null);
        
        var prefab = new CustomPrefab(prefabInfo);
        var cloneTemplate = new CloneTemplate(prefabInfo, "8b5e6a02-533c-44cb-9f34-d2773aa82dc4");
        
        prefab.SetGameObject(cloneTemplate);

        prefab.Register();
    }
}