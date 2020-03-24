using System.Collections;
using UnityEngine;
using Entities;

public class PlantSpawner : MonoBehaviour
{
    [SerializeField]
    [Range(0, 1)] private float _chanceToGrowPerSecond = 0.2f;

    [SerializeField]
    private LivingBeing _plantPrefab = null;

    [SerializeField]
    private float _growSpeed = 0.33f;

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
            if (Random.Range(0f, 1f) < _chanceToGrowPerSecond &&
                SuitablePositionFound())
            {
                LivingBeing newPlant = Instantiate(_plantPrefab, _newWorldPos, Quaternion.identity);
                newPlant.transform.parent = transform;
                Collider newPlantCollider = newPlant.GetComponent<Collider>();
                newPlantCollider.enabled = false;

                StartCoroutine(GrowPlant(newPlant, newPlantCollider));
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

            if (_grid.NodeFromWorldInput(_newWorldPos).Walkable &&
                !ObstacleInArea(_newWorldPos))
                return true;
        }

        return false;
    }

    private IEnumerator GrowPlant(LivingBeing targetPlant, Collider targetPlantCollider)
    {
        Vector3 defaultScale = targetPlant.transform.localScale;
        targetPlant.transform.localScale = Vector3.zero;
        targetPlant.transform.position -= Vector3.up * 0.5f * defaultScale.y; 

        float t = 0;
        while (Vector3.Distance(defaultScale, targetPlant.transform.localScale) > Mathf.Epsilon)
        {
            yield return null;
            t += _growSpeed * Time.deltaTime;
            targetPlant.transform.localScale = Vector3.Lerp(Vector3.zero, defaultScale, t);
            targetPlant.transform.position += Vector3.up * (Mathf.Lerp(1f - 0.5f * defaultScale.y, 1f, t) - targetPlant.transform.position.y);
        }
        targetPlantCollider.enabled = true;
    }

    private bool ObstacleInArea(Vector3 pos)
    {
        return Physics.OverlapSphere(pos, 1.5f, LayerMask.GetMask("Obstacle")).Length > 0;
    }
}
