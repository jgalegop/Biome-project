using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities;

public class PlantSpawner : MonoBehaviour, IDisplayableText
{
    [SerializeField]
    [Tooltip("Chance per second to create a plant")]
    [Range(0, 1)] private float _plantsPerSecond = 0.0f;

    [SerializeField]
    private LivingBeing _plantPrefab = null;

    [SerializeField]
    private float _growSpeed = 0.33f;

    private PathfindGrid _grid = null;

    private readonly int findPosTries = 10;

    private Vector3 _newWorldPos;
    private List<Coroutine> _runningCoroutines = new List<Coroutine>();

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        _grid = FindObjectOfType<PathfindGrid>();
        if (_grid == null)
            Debug.LogError("Scene needs a PathfindGrid object");

        Coroutine growingPlants = StartCoroutine(ChanceToGrowPlant());
        _runningCoroutines.Add(growingPlants);
    }

    private IEnumerator ChanceToGrowPlant()
    {
        while (true)
        {
            float averagePlantTime = 1f / (_plantsPerSecond + Mathf.Epsilon);
            float nextPlantTime = Random.Range(0.8f * averagePlantTime, 1.2f * averagePlantTime);
            yield return new WaitForSeconds(nextPlantTime);
            if (SuitablePositionFound())
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
            targetPlant.transform.position += Vector3.up * (targetPlant.GroundYPos + 0.5f * targetPlant.transform.localScale.y - targetPlant.transform.position.y);
        }
        targetPlantCollider.enabled = true;
    }

    private bool ObstacleInArea(Vector3 pos)
    {
        return Physics.OverlapSphere(pos, 1.5f, LayerMask.GetMask("Obstacle")).Length > 0;
    }

    public void SetPlantsPerSecond(float pps)
    {
        _plantsPerSecond = Mathf.Clamp(pps, 0f, 2f);
        StopPlantSpawning();
        Coroutine growingPlants = StartCoroutine(ChanceToGrowPlant());
        _runningCoroutines.Add(growingPlants);
    }

    private void StopPlantSpawning()
    {
        // this makes sure there is no multiple instances of the plant spawning coroutine
        foreach (Coroutine co in _runningCoroutines)
        {
            StopCoroutine(co);
        }
        _runningCoroutines = new List<Coroutine>();
    }

    public T SetText<T>()
    {
        //Debug.Log("plants " + _plantsPerSecond);
        return (T)System.Convert.ChangeType(_plantsPerSecond, typeof(T));
    }
}
