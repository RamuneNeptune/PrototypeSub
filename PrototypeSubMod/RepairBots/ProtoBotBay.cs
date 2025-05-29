using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PrototypeSubMod.RepairBots;

internal class ProtoBotBay : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform pathfindingManager;
    [SerializeField] private Transform elevatorTransform;

    private Queue<CyclopsDamagePoint> damagePoints = new();
    private ProtoRepairBot repairBot;
    private Quaternion stowedRot;
    private Vector3 stowedPos;
    private bool botActive;

    private IEnumerator Start()
    {
        // The extra frame waits ensure that the bots are spawned even if it's already initialized
        yield return new WaitUntil(() => SpawnRepairBot.Initialized);
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        repairBot = elevatorTransform.GetComponentInChildren<ProtoRepairBot>(true);
        repairBot.gameObject.SetActive(false);
        repairBot.SetOwnerBay(this);

        stowedPos = repairBot.transform.localPosition;
        stowedRot = repairBot.transform.localRotation;
    }

    public void QueueBotDeployment(CyclopsDamagePoint targetPoint)
    {
        damagePoints.Enqueue(targetPoint);

        if (!botActive)
        {
            StartCoroutine(DeployBotAsync());
        }
    }

    private IEnumerator DeployBotAsync()
    {
        botActive = true;
        animator.SetBool("Opened", true);
        repairBot.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.83f);

        repairBot.transform.SetParent(pathfindingManager);
        repairBot.UpdateUseLocalPos();

        var damagePoint = damagePoints.Dequeue();
        repairBot.SetRepairPoint(damagePoint);
        repairBot.SetEnRouteToPoint();
        repairBot.UpdatePath(damagePoint.transform.position + (damagePoint.transform.forward * 0.25f));
    }

    public void OnPointRepaired()
    {
        if (damagePoints.Count > 0)
        {
            var damagePoint = damagePoints.Dequeue();
            repairBot.SetRepairPoint(damagePoint);
            repairBot.SetEnRouteToPoint();
            repairBot.UpdatePath(damagePoint.transform.position + (damagePoint.transform.forward * 0.25f));
        }
        else
        {
            repairBot.UpdatePath(elevatorTransform.position);
        }
    }

    public void OnReturnToElevator()
    {
        StartCoroutine(StowBot());
    }

    private IEnumerator StowBot()
    {
        botActive = false;
        repairBot.transform.SetParent(elevatorTransform, false);
        repairBot.transform.localPosition = stowedPos;
        repairBot.transform.localRotation = stowedRot;
        repairBot.ResetVisualRotation();

        animator.SetBool("Opened", false);

        yield return new WaitForSeconds(1f);
        repairBot.gameObject.SetActive(false);
    }
}
