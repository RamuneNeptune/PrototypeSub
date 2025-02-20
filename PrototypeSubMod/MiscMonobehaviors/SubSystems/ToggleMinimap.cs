using System.Collections;
using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.SubSystems;

internal class ToggleMinimap : MonoBehaviour
{
    [SerializeField] private GameObject positionDisplay;
    [SerializeField] private int maxSpawnWaitFrames = 10;

    private int frameCount;
    private MiniWorld miniWorld;

    private IEnumerator Start()
    {
        positionDisplay.SetActive(false);

        while (frameCount < maxSpawnWaitFrames)
        {
            yield return new WaitForEndOfFrame();

            var world = gameObject.GetComponentInChildren<MiniWorld>();
            if (world)
            {
                miniWorld = world;
                miniWorld.active = false;
                yield break;
            }

            frameCount++;
        }

        Plugin.Logger.LogError($"Mini world not found as a child of {gameObject} after {maxSpawnWaitFrames} frames");
    }

    public void ToggleMap()
    {
        if (!miniWorld) return;

        miniWorld.ToggleMap();
        positionDisplay.SetActive(!positionDisplay.activeSelf);
    }
}
