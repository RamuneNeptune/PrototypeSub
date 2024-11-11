using Nautilus.Assets;
using Nautilus.Assets.PrefabTemplates;
using PrototypeSubMod.Monobehaviors;
using Story;

namespace PrototypeSubMod.Prefabs.FacilityProps;

internal class TeleporterTerminal_World
{
    public static PrefabInfo prefabInfo { get; private set; }

    public static void Register()
    {
        prefabInfo = PrefabInfo.WithTechType("InterceptorDataTerminal", null, null);

        var prefab = new CustomPrefab(prefabInfo);

        var cloneTemplate = new CloneTemplate(prefabInfo, "d200d747-b802-43f4-80b1-5c3d2155fbcd");

        cloneTemplate.ModifyPrefab += gameObject =>
        {
            var handTarget = gameObject.GetComponent<StoryHandTarget>();
            handTarget.goal = new StoryGoal("OnInterceptorTestDataDownloaded", Story.GoalType.PDA, 0f);

            gameObject.EnsureComponent<TeleporterDisablerBehavior>();
        };

        prefab.SetGameObject(cloneTemplate);

        prefab.Register();
    }
}
