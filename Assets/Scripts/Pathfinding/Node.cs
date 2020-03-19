using UnityEngine;

public class Node
{
    public bool Walkable;
    public Vector3 WorldPos;

    public int GridX;
    public int GridY;

    /// <summary>
    /// Distance from starting node to node
    /// </summary>
    public int GCost;

    /// <summary>
    /// Distance from target node to node
    /// </summary>
    public int HCost;

    public int FCost
    {
        get
        {
            return GCost + HCost;
        }
    }

    /// <summary>
    /// Node from which this Node is neighbour
    /// </summary>
    public Node Parent;

    public Node(bool walkable, Vector3 worldPos, int gridX, int gridY)
    {
        Walkable = walkable;
        WorldPos = worldPos;
        GridX = gridX;
        GridY = gridY;
    }
}