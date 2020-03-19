using UnityEngine;

public class Node : IHeapItem<Node>
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

    private int _heapIndex;



    public Node(bool walkable, Vector3 worldPos, int gridX, int gridY)
    {
        Walkable = walkable;
        WorldPos = worldPos;
        GridX = gridX;
        GridY = gridY;
    }

    public int HeapIndex
    {
        get
        {
            return _heapIndex;
        }
        set
        {
            _heapIndex = value;
        }
    }

    public int CompareTo(Node nodeToCompare)
    {
        int compare = FCost.CompareTo(nodeToCompare.FCost);
        if (compare == 0)
        {
            compare = HCost.CompareTo(nodeToCompare.HCost);
        }
        return - compare;
    }
}