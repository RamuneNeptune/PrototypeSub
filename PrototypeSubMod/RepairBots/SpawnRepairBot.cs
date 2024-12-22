﻿using PrototypeSubMod.Pathfinding;
using PrototypeSubMod.Prefabs;
using System.Collections;
using UnityEngine;

namespace PrototypeSubMod.RepairBots;

internal class SpawnRepairBot : MonoBehaviour
{
    public static bool Initialized
    {
        get
        {
            return botPrefab != null;
        }
    }

    private static GameObject botPrefab;

    [SerializeField] private PathRequestManager requestManager;

    private IEnumerator Start()
    {
        if (botPrefab != null)
        {
            SpawnBot();
            yield break;
        }

        var botTask = CraftData.GetPrefabForTechTypeAsync(ProtoRepairBot_Spawned.prefabInfo.TechType);
        yield return botTask;

        botPrefab = botTask.GetResult();
        SpawnBot();
    }

    private void SpawnBot()
    {
        var newBot = Instantiate(botPrefab, transform.parent);

        newBot.transform.SetParent(transform.parent);
        newBot.transform.localPosition = transform.localPosition;
        newBot.transform.localRotation = transform.localRotation;

        var repairBot = newBot.GetComponent<ProtoRepairBot>();
        repairBot.SetBotLocalPos();
        repairBot.SetPathfindingManager(requestManager);

        newBot.gameObject.SetActive(false);

        Destroy(gameObject);
    }
}
