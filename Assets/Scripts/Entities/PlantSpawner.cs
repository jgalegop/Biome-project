using System.Collections;
using UnityEngine;
using Entities;

public class PlantSpawner : MonoBehaviour
{
    [SerializeField]
    [Range(0, 1)] private float _chanceToGrowPerSecond = 0.2f;

    [SerializeField]
    private LivingBeing _plantPrefab = null;

    private PathfindGrid _grid = null;

    private int findPosTries = 10;

    private Vector3 _newWorldPos;

    private void Start()
    {
        _grid = FindObjectOfType<PathfindGrid>();
        if (_grid == null)
            Debug.LogError("Scene needs a PathfindGrid object");

        StartCoroutine(ChanceToGrowPlant());
    }

    private IEnumerator ChanceToGrowPlant()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (Random.Range(0f, 1f) < _chanceToGrowPerSecond)
            {
                if (SuitablePositionFound())
                {
                    LivingBeing newPlant = Instantiate(_plantPrefab, _newWorldPos, Quaternion.identity);
                    newPlant.transform.parent = transform;
                }
            }
        }
    }

    private bool SuitablePositionFound()
    {
        for (int i = 0; i < findPosTries; i++)
        {
            float newX = Random.Range(-0.5f * _grid.GridWorldSize.x, +0.5f * _grid.GridWorldSize.x);
            float newY = Random.Range(-0.5f * _grid.GridWorldSize.y, +0.5f * _grid.GridWorldSize.y);
            _newWorldPos = new Vector3(newX, 1, newY);

            Node posNode = _grid.NodeFromWorldInput(_newWorldPos);
            if (posNode.Walkable)
            {
                return true;
            }
        }

        return false;
    }
}
