﻿using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace PrototypeSubMod.Pathfinding;

public class PathRequestManager : MonoBehaviour
{
    [SerializeField] private Pathfinder pathfinder;

    private Queue<PathResult> results = new Queue<PathResult>();

    private void Update()
    {
        if (results.Count > 0)
        {
            int itemsInQueue = results.Count;
            lock (results)
            {
                for (int i = 0; i < itemsInQueue; i++)
                {
                    var result = results.Dequeue();
                    result.callback(result.path, result.success);
                }
            }
        }
    }

    public void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<PathData[], bool> callback)
    {
        RequestPath(new PathRequest(pathStart, pathEnd, callback));
    }
    
    public void RequestPath(PathRequest request)
    {
        if (!pathfinder.pathfindingGrid.initialized || pathfinder.pathfindingGrid.GetGrid() == null)
        {
            request.callback(Array.Empty<PathData>(), false);
            Plugin.Logger.LogWarning("Grid not initialized. Returning");
            return;
        }
        
        var startNode = pathfinder.pathfindingGrid.GetNodeAtWorldPosition(request.pathStart);
        var endNode = pathfinder.pathfindingGrid.GetNodeAtWorldPosition(request.pathEnd);
        ThreadStart threadStart = () =>
        {
            pathfinder.QueuePathTrace(request, FinishedProcessingPath, startNode, endNode);
        };
        
        var thread = new Thread(threadStart);
        thread.Start();
    }

    public void FinishedProcessingPath(PathResult result)
    {
        lock (results)
        {
            //Plugin.Logger.LogInfo("Enqueuing results");
            results.Enqueue(result);
        }
    }
}

public struct PathRequest
{
    public Vector3 pathStart;
    public Vector3 pathEnd;
    public Action<PathData[], bool> callback;

    public PathRequest(Vector3 pathStart, Vector3 pathEnd, Action<PathData[], bool> callback)
    {
        this.pathStart = pathStart;
        this.pathEnd = pathEnd;
        this.callback = callback;
    }
}

public struct PathResult
{
    public PathData[] path;
    public bool success;
    public Action<PathData[], bool> callback;

    public PathResult(PathData[] path, bool success, Action<PathData[], bool> callback)
    {
        this.path = path;
        this.success = success;
        this.callback = callback;
    }
}