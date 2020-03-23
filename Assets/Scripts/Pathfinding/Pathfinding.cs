using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PathfindGrid))]
public class Pathfinding : MonoBehaviour
{
    private PathfindGrid _grid;
    private PathRequestManager _requestManager;

    private void Awake()
    {
        _grid = GetComponent<PathfindGrid>();
        _requestManager = GetComponent<PathRequestManager>();
    }


    public void StartFindPath(Vector3 pathStart, Vector3 pathEnd)
    {
        StartCoroutine(FindPath(pathStart, pathEnd));
    }


    IEnumerator FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        Node startNode = _grid.NodeFromWorldInput(startPos);
        Node targetNode = _grid.NodeFromWorldInput(targetPos);

        if (startNode.Walkable && targetNode.Walkable)
        {
            Heap<Node> openSet = new Heap<Node>(_grid.MaxSize);
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst();

                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    pathSuccess = true;
                    break;
                }

                foreach (Node neighbour in _grid.GetNeighbours(currentNode))
                {
                    if (!neighbour.Walkable || closedSet.Contains(neighbour))
                        continue;

                    int newMoveCostToNeighbour = currentNode.GCost + GetDistance(currentNode, neighbour);
                    if (newMoveCostToNeighbour < neighbour.GCost ||
                        !openSet.Contains(neighbour))
                    {
                        neighbour.GCost = newMoveCostToNeighbour;
                        neighbour.HCost = GetDistance(neighbour, targetNode);
                        neighbour.Parent = currentNode;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                        else
                            openSet.UpdateItem(neighbour);
                    }
                }
            }
        }

        yield return null;
        if (pathSuccess)
            waypoints = RetracePath(startNode, targetNode);

        _requestManager.FinishedProcessingPath(waypoints, pathSuccess);
    }

    private int GetDistance(Node nodeA, Node nodeB)
    {
        int distX = Mathf.Abs(nodeA.GridX - nodeB.GridX);
        int distY = Mathf.Abs(nodeA.GridY - nodeB.GridY);

        return 14 * Mathf.Min(distX, distY) + 10 * Mathf.Abs(distY - distX);
    }

    private Vector3[] RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;
        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.Parent;
        }
        path.Add(startNode); // -----------------------> this??
        Vector3[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);
        return waypoints;
    }

    private Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;
        for (int i = 1; i < path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i - 1].GridX - path[i].GridX, path[i - 1].GridY - path[i].GridY);
            if (directionNew != directionOld)
            {
                waypoints.Add(path[i - 1].WorldPos);
            }
            directionOld = directionNew;
        }
        if (waypoints.Count == 0) // if target is adyacent
        {
            waypoints.Add(path[0].WorldPos);
        }
        return waypoints.ToArray();
    }
}
