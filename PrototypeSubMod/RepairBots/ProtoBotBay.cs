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

    private ProtoRepairBot repairBot;
    private bool botActive;

    private IEnumerator Start()
    {
        // The extra frame waits ensure that the bots are spawned even if it's already initialized
        yield return new WaitUntil(() => SpawnRepairBot.Initialized);
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        repairBot = elevatorTransform.GetComponentInChildren<ProtoRepairBot>();
        repairBot.gameObject.SetActive(false);
    }

    public void DeployBot(CyclopsDamagePoint targetPoint)
    {
        if (botActive)
        {
            Plugin.Logger.LogError($"Attempted to deploy bot on {gameObject} while it was already deployed");
            return;
        }

        StartCoroutine(DeployBotAsync(targetPoint));
    }

    private IEnumerator DeployBotAsync(CyclopsDamagePoint targetPoint)
    {
        animator.SetBool("Opened", true);
        repairBot.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.83f);

        animator.SetBool("Opened", false);
        repairBot.transform.SetParent(pathfindingManager);
        repairBot.SetReturnPoint(returnPos);
        repairBot.UpdateUseLocalPos();
        repairBot.UpdatePath(targetPoint.transform.position);
    }
}
