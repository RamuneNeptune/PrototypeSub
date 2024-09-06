using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

using Debug = UnityEngine.Debug;

namespace PrototypeSubMod.Pathfinding;

public class Pathfinder : MonoBehaviour
{
    public PathfindingGrid pathfindingGrid;

    private Heap<GridNode> openSet;
    private Heap<GridNode> closedSet;

    public void FindPath(PathRequest request, Action<PathResult> callback)
    {
        if (openSet == null)
        {
            openSet = new Heap<GridNode>(pathfindingGrid.GetMaxSize());
        }

        if (closedSet == null)
        {
            closedSet = new Heap<GridNode>(pathfindingGrid.GetMaxSize());
        }

        Stopwatch sw = new Stopwatch();
        sw.Start();

        PathData[] waypoints = new PathData[0];
        bool pathSuccess = false;

        GridNode startNode = pathfindingGrid.GetNodeAtWorldPosition(request.pathStart);
        GridNode endNode = pathfindingGrid.GetNodeAtWorldPosition(request.pathEnd);
        GridNode lowestHCostNode = new GridNode();
        lowestHCostNode.hCost = int.MaxValue;

        if (!startNode.walkable && !endNode.walkable)
        {
            callback(new PathResult(waypoints, pathSuccess, request.callback));
            //Plugin.Logger.LogInfo("Invalid nodes. Returning");
            return;
        }

        openSet.Clear();
        closedSet.Clear();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            GridNode currentNode = openSet.RemoveFirstItem();
            closedSet.Add(currentNode);

            if (currentNode == endNode)
            {
                sw.Stop();
                //Debug.Log($"Path found in {sw.ElapsedMilliseconds}ms");

                pathSuccess = true;
                break;
            }

            foreach (GridNode neighbor in pathfindingGrid.GetAdjacentNodes(currentNode))
            {
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

        //Debug.Log($"Path success = {pathSuccess}");

        sw.Stop();

        callback(new PathResult(waypoints, pathSuccess, request.callback));
    }

    private PathData[] RetracePath(GridNode startNode, GridNode endNode)
    {
        List<GridNode> path = new List<GridNode>();
        GridNode currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);

            if (currentNode.parent == null) break;
            currentNode = currentNode.parent;
        }

        PathData[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);

        return waypoints;
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
}
