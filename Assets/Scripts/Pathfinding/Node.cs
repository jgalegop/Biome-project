using UnityEngine;

public struct Node
{
    public bool Walkable;
    public Vector3 WorldPos;

    public Node(bool walkable, Vector3 worldPos)
    {
        Walkable = walkable;
        WorldPos = worldPos;
    }
}