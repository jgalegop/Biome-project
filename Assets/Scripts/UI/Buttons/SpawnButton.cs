using UnityEngine;

public class SpawnButton : Button
{
    [SerializeField]
    private GameObject _spawnPrefab = null;
    [SerializeField]
    private GameObject _prefabParent = null;

    [SerializeField]
    private SpawnNumberController _spawnNumberController = null;

    private PathfindGrid _grid = null;

    private int _findPosTries = 10;
    private Vector3 _newWorldPos = Vector3.zero;

    public override void Awake()
    {
        base.Awake();
        _grid = FindObjectOfType<PathfindGrid>();
        if (_grid == null)
            Debug.LogError("Scene needs a PathfindGrid object");
    }

    public void SpawnPrefab()
    {
        int k = 0;
        while (k < _spawnNumberController.SpawnNumber)
        {
            if (SuitablePositionFound())
            {
                GameObject go = Instantiate(_spawnPrefab, _newWorldPos, Quaternion.identity);
                go.transform.SetParent(_prefabParent.transform);
                k++;
                if (go.layer == LayerMask.NameToLayer("Obstacle"))
                {
                    _grid.StartPathfindGrid();
                }
            }
            else
            {
                Debug.Log("No suitable position found!");
            }
        }
        
    }

    private bool SuitablePositionFound()
    {
        for (int i = 0; i < _findPosTries; i++)
        {
            float newX = Random.Range(-0.5f * _grid.GridWorldSize.x, +0.5f * _grid.GridWorldSize.x);
            float newY = Random.Range(-0.5f * _grid.GridWorldSize.y, +0.5f * _grid.GridWorldSize.y);
            _newWorldPos = new Vector3(newX, 1, newY);

            if (_grid.NodeFromWorldInput(_newWorldPos).Walkable &&
                !ObstacleInArea(_newWorldPos))
                return true;
        }

        return false;
    }

    private bool ObstacleInArea(Vector3 pos)
    {
        return Physics.OverlapSphere(pos, 1.5f, LayerMask.GetMask("Obstacle")).Length > 0;
    }
}
