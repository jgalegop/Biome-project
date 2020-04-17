using UnityEngine;

public class SpawnButton : MonoBehaviour
{
    [SerializeField]
    private GameObject _spawnPrefab = null;
    [SerializeField]
    private GameObject _prefabParent = null;

    private PathfindGrid _grid = null;

    private int _findPosTries = 10;
    private Vector3 _newWorldPos = Vector3.zero;

    private void Awake()
    {
        _grid = FindObjectOfType<PathfindGrid>();
        if (_grid == null)
            Debug.LogError("Scene needs a PathfindGrid object");
    }

    public void SpawnPrefab()
    {
        if (SuitablePositionFound())
        {
            GameObject go = Instantiate(_spawnPrefab, _newWorldPos, Quaternion.identity);
            go.transform.SetParent(_prefabParent.transform);

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

    public void ChangeSizeOnEnter()
    {
        transform.localScale *= 1.1f;
    }

    public void ChangeSizeOnExit()
    {
        transform.localScale *= 1/1.1f;
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
