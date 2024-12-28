using Story;
using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors;

internal class StoryGoalTrigger : MonoBehaviour
{
    [SerializeField] private string key;

    public void Trigger()
    {
        StoryGoalManager.main.OnGoalComplete(key);
    }
}
