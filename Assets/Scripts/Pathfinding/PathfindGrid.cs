using UnityEngine;

public class PathfindGrid : MonoBehaviour
{
    public LayerMask _unwalkableMask;

    [SerializeField]
    private Vector2 _gridWorldSize = new Vector3(1, 1, 1);

    [SerializeField]
    private float _nodeRadius = 1f;

    private Node[,] grid;
    private float _nodeDiameter;
    private int _gridSizeX, _gridSizeY;

    private void Start()
    {
        transform.position = Vector3.zero;

        _nodeDiameter = 2 * _nodeRadius;
        _gridSizeX = Mathf.RoundToInt(_gridWorldSize.x / _nodeDiameter);
        _gridSizeY = Mathf.RoundToInt(_gridWorldSize.y / _nodeDiameter);
        CreateGrid();
    }

    private void CreateGrid()
    {
        grid = new Node[_gridSizeX, _gridSizeY];
        Vector3 worldBottomLeft = transform.position + 0.5f * _gridWorldSize.x * Vector3.left
                                                     + 0.5f * _gridWorldSize.y * Vector3.back;

        for (int x = 0; x < _gridSizeX; x++)
        {
            for (int y = 0; y < _gridSizeX; y++)
            {
                Vector3 worldNodePoint = worldBottomLeft + (x * _nodeDiameter + _nodeRadius) * Vector3.right
                                                         + (y * _nodeDiameter + _nodeRadius) * Vector3.forward;
                bool walkable = !Physics.CheckBox(worldNodePoint, _nodeRadius * Vector3.one, Quaternion.identity, _unwalkableMask);
                grid[x, y] = new Node(walkable, worldNodePoint);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(_gridWorldSize.x, 5, _gridWorldSize.y));

        if (grid != null)
        {
            foreach (Node node in grid)
            {
                Gizmos.color = node.Walkable ? Color.white : Color.red;
                Gizmos.DrawWireCube(node.WorldPos, _nodeDiameter * 0.95f * Vector3.one);
            }
        }
    }
}
