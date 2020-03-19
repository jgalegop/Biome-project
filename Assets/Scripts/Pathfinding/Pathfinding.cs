using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PathfindGrid))]
public class Pathfinding : MonoBehaviour
{

    public Transform seeker;
    public Transform target;


    private PathfindGrid _grid => GetComponent<PathfindGrid>();

    private void Update()
    {
        FindPath(seeker.position, target.position);
    }

    private void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = _grid.NodeFromWorldInput(startPos);
        Node targetNode = _grid.NodeFromWorldInput(targetPos);

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FCost < currentNode.FCost ||
                    openSet[i].FCost == currentNode.FCost && openSet[i].HCost < currentNode.HCost)
                {
                    currentNode = openSet[i];
                }
            }
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                RetracePath(startNode, targetNode);
                return;
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
                }
            }
        }
    }

    private int GetDistance(Node nodeA, Node nodeB)
    {
        int distX = Mathf.Abs(nodeA.GridX - nodeB.GridX);
        int distY = Mathf.Abs(nodeA.GridY - nodeB.GridY);

        return 14 * Mathf.Min(distX, distY) + 10 * Mathf.Abs(distY - distX);
    }

    private void RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;
        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.Parent;
        }
        path.Reverse();

        _grid.Path = path;
    }
}
