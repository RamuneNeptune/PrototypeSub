using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace PrototypeSubMod.Pathfinding;

public class Pathfinder : MonoBehaviour
{
    public PathfindingGrid pathfindingGrid;

    private Queue<PathQueueData> queueDatas = new();
    
    public void QueuePathTrace(PathRequest request, Action<PathResult> callback, GridNode startNode, GridNode endNode)
    {
        PathQueueData queueData = new PathQueueData(request, callback, startNode, endNode);

        queueDatas.Enqueue(queueData);
        
        while (queueDatas.Count > 0)
        {
            var data = queueDatas.Dequeue();
            FindPath(data.request, data.callback, data.startNode, data.endNode);
        }
    }
    
    public void FindPath(PathRequest request, Action<PathResult> callback, GridNode startNode, GridNode endNode)
    {
        PathData[] waypoints = Array.Empty<PathData>();
        bool pathSuccess = false;
        
        Heap<GridNode> openSet;
        Heap<GridNode> closedSet;
        lock (pathfindingGrid)
        {
            openSet = new Heap<GridNode>(pathfindingGrid.GetMaxSize());
            closedSet = new Heap<GridNode>(pathfindingGrid.GetMaxSize());
        }
        //Plugin.Logger.LogInfo("Heaps created");
        GridNode lowestHCostNode = new GridNode
        {
            hCost = int.MaxValue
        };
        
        if (!startNode.walkable && !endNode.walkable)
        {
            callback(new PathResult(waypoints, pathSuccess, request.callback));
            Plugin.Logger.LogWarning("Invalid nodes. Returning");
            return;
        }
        
        openSet.Add(startNode);
        
        while (openSet.Count > 0)
        {
            GridNode currentNode = openSet.RemoveFirstItem();
            closedSet.Add(currentNode);
            
            if (SameNode(currentNode, endNode))
            {
                pathSuccess = true;
                break;
            }
            
            List<GridNode> nodes;
            nodes = pathfindingGrid.GetAdjacentNodes(currentNode);
            
            for (int i = 0; i < nodes.Count; i++)
            {
                var neighbor = nodes[i];
                if (!neighbor.walkable || closedSet.Contains(neighbor)) continue;
                int newMoveCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor) + neighbor.movementPenalty;

                if (newMoveCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newMoveCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, endNode);
                    neighbor.parent = currentNode;
                    
                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                    else
                    {
                        openSet.UpdateItem(neighbor);
                    }

                    if (neighbor.hCost < lowestHCostNode.hCost)
                    {
                        lowestHCostNode = neighbor;
                    }
                }
            }
        }

        if (pathSuccess)
        {
            waypoints = RetracePath(startNode, endNode);
            pathSuccess = waypoints.Length > 0;
        }
        else
        {
            waypoints = RetracePath(startNode, lowestHCostNode);
        }

        callback(new PathResult(waypoints, pathSuccess, request.callback));
    }

    private PathData[] RetracePath(GridNode startNode, GridNode endNode)
    {
        List<GridNode> path = new List<GridNode>();
        GridNode currentNode = endNode;

        while (!SameNode(currentNode, startNode))
        {
            path.Add(currentNode);

            if (currentNode.parent == null) break;
            currentNode = currentNode.parent;
        }

        PathData[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);

        return waypoints;
    }

    private bool SameNode(GridNode nodeA, GridNode nodeB)
    {
        Vector3Int nodeAPos = new Vector3Int(nodeA.gridPosX, nodeA.gridPosY, nodeA.gridPosZ);
        Vector3Int nodeBPos = new  Vector3Int(nodeB.gridPosX, nodeB.gridPosY, nodeB.gridPosZ);
        return nodeAPos == nodeBPos;
    }

    private PathData[] SimplifyPath(List<GridNode> path)
    {
        List<PathData> waypoints = new List<PathData>();
        Vector3 directionOld = Vector3.zero;
        waypoints.Add(new PathData(path[0].pointOnBounds.Vector, path[0].surfaceNormal.Vector));

        for (int i = 1; i < path.Count; i++)
        {
            /*
            waypoints.Add(new PathData(path[i].pointOnBounds, path[i].surfaceNormal));
            continue;
            */

            Vector3 directionNew = path[i].pointOnBounds.Vector - path[i - 1].pointOnBounds.Vector;
            if (directionNew != directionOld)
            {
                waypoints.Add(new PathData(path[i].pointOnBounds.Vector, path[i].surfaceNormal.Vector));
            }

            directionOld = directionNew;
        }

        return waypoints.ToArray();
    }

    private int GetDistance(GridNode nodeA, GridNode nodeB)
    {
        int distX = Mathf.Abs(nodeA.gridPosX - nodeB.gridPosX);
        int distY = Mathf.Abs(nodeA.gridPosY - nodeB.gridPosY);
        int distZ = Mathf.Abs(nodeA.gridPosZ - nodeB.gridPosZ);

        if (distZ > distX) Swap(ref distX, ref distZ);
        if (distY > distX) Swap(ref distX, ref distY);
        if (distY > distZ) Swap(ref distZ, ref distY);

        return (17 * distY) + (14 * (distZ - distY)) + (10 * (distX - distZ));
    }

    private void Swap(ref int a, ref int b)
    {
        (a, b) = (b, a);
    }

    public struct PathQueueData
    {
        public PathRequest request;
        public Action<PathResult> callback;
        public GridNode startNode;
        public GridNode endNode;

        public PathQueueData(PathRequest request, Action<PathResult> callback, GridNode startNode, GridNode endNode)
        {
            this.request = request;
            this.callback = callback;
            this.startNode = startNode;
            this.endNode = endNode;
        }
    }
}
