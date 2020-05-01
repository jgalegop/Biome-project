using UnityEngine;
using System.Collections.Generic;

public class PathfindGrid : MonoBehaviour
{
    public LayerMask _unwalkableMask;

    public Vector2 GridWorldSize = new Vector2(10, 10);

    [SerializeField]
    private float _nodeRadius = 1f;

    [SerializeField]
    private bool _displayGridGizmos = true;

    private Node[,] grid;
    private float _nodeDiameter;
    private int _gridSizeX, _gridSizeY;

    private void Awake()
    {
        StartPathfindGrid();
    }

    public void StartPathfindGrid()
    {
        transform.position = Vector3.zero;

        _nodeDiameter = 2 * _nodeRadius;

        GameObject terrain = GameObject.Find("TerrainMesh");
        if (terrain == null)
            Debug.LogError("We need a terrain mesh first. Make sure that you have it in-game, or that this script is not called in awake");

        Vector3 terrainSize = terrain.GetComponent<MeshFilter>().sharedMesh.bounds.size;
        GridWorldSize = new Vector2(terrainSize.x, terrainSize.z);

        _gridSizeX = Mathf.RoundToInt(GridWorldSize.x / _nodeDiameter);
        _gridSizeY = Mathf.RoundToInt(GridWorldSize.y / _nodeDiameter);

        CreateGrid();
    }

    private void CreateGrid()
    {
        grid = new Node[_gridSizeX, _gridSizeY];
        Vector3 worldBottomLeft = transform.position + 0.5f * GridWorldSize.x * Vector3.left
                                                     + 0.5f * GridWorldSize.y * Vector3.back;

        for (int x = 0; x < _gridSizeX; x++)
        {
            for (int y = 0; y < _gridSizeY; y++)
            {
                Vector3 worldNodePoint = worldBottomLeft + (x * _nodeDiameter + _nodeRadius) * Vector3.right
                                                         + (y * _nodeDiameter + _nodeRadius) * Vector3.forward;
                bool walkable = !Physics.CheckBox(worldNodePoint, _nodeRadius * Vector3.one, Quaternion.identity, _unwalkableMask);
                grid[x, y] = new Node(walkable, worldNodePoint, x, y);
            }
        }
    }

    public int MaxSize
    {
        get
        {
            return _gridSizeX * _gridSizeY;
        }
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int neighbourX = node.GridX + x;
                int neighbourY = node.GridY + y;

                if (neighbourX >= 0 && neighbourX < _gridSizeX &&
                    neighbourY >= 0 && neighbourY < _gridSizeY)
                {
                    neighbours.Add(grid[neighbourX, neighbourY]);
                }
            }
        }

        return neighbours;
    }

    public Node NodeFromWorldInput(Vector3 worldPos)
    {
        float percentX = (worldPos.x + 0.5f * GridWorldSize.x) / GridWorldSize.x;
        float percentY = (worldPos.z + 0.5f * GridWorldSize.y) / GridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt(percentX * (_gridSizeX - 1));
        int y = Mathf.RoundToInt(percentY * (_gridSizeY - 1));
        return grid[x, y];
    }

    private void OnDrawGizmos()
    {
        if (grid != null && _displayGridGizmos)
        {
            foreach (Node node in grid)
            {
                Gizmos.color = node.Walkable ? Color.white : Color.red;
                Gizmos.DrawCube(node.WorldPos + Vector3.down * 0f, 
                    _nodeDiameter * 0.9f * Vector3.one - _nodeDiameter * 0.95f * Vector3.up);
            }
        }
    }
}
