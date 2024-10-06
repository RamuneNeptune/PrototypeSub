using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PrototypeSubMod.RepairBots;

internal class ProtoBotBay : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform pathfindingManager;
    [SerializeField] private Transform elevatorTransform;
    [SerializeField] private Transform returnPos;

    private List<ProtoRepairBot> activeBots = new();
    private Queue<ProtoRepairBot> inactiveBots = new();

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => SpawnRepairBot.Initialized);
        yield return new WaitForEndOfFrame();

        foreach (var bot in elevatorTransform.GetComponentsInChildren<ProtoRepairBot>())
        {
            inactiveBots.Enqueue(bot);
        }
    }

    public void DeployBot(CyclopsDamagePoint targetPoint)
    {
        if (inactiveBots.Count <= 0)
        {
            Plugin.Logger.LogError($"Out of repair bots on {gameObject}!");
            return;
        }

        StartCoroutine(DeployBotAsync(targetPoint));
    }

    private IEnumerator DeployBotAsync(CyclopsDamagePoint targetPoint)
    {
        animator.SetBool("Opened", true);
        var deployedBot = inactiveBots.Dequeue();
        activeBots.Add(deployedBot);

        yield return new WaitForSeconds(0.83f);

        animator.SetBool("Opened", false);
        deployedBot.transform.SetParent(pathfindingManager);
        deployedBot.SetTargetPoint(targetPoint, returnPos);
    }
}
