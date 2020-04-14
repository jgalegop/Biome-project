using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities;

public class AnimalSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _animalPrefabsList = null;

    [SerializeField]
    private int[] _initialNumberOfAnimals = null;

    private PathfindGrid _grid = null;
    private Vector3 _newWorldPos = Vector3.zero;
    private int findPosTries = 10;

    private void Awake()
    {
        Init();
    }

    private void OnEnable()
    {
        Init();   
    }

    private void Init()
    {
        if (_animalPrefabsList.Length != _initialNumberOfAnimals.Length)
            Debug.LogError("Lists must be the same size");

        _grid = FindObjectOfType<PathfindGrid>();
        if (_grid == null)
            Debug.LogError("Scene needs a PathfindGrid object");
    }


    private void Start()
    {
        int k = 0;
        foreach(GameObject prefab in _animalPrefabsList)
        {
            int animalNumber = _initialNumberOfAnimals[k];
            for (int i = 0; i < animalNumber; i++)
            {
                while (!SuitablePositionFound()) { }

                GameObject animal = Instantiate(prefab, _newWorldPos, Quaternion.identity);
                animal.transform.SetParent(transform.parent);
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

    private bool ObstacleInArea(Vector3 pos)
    {
        return Physics.OverlapSphere(pos, 1.5f, LayerMask.GetMask("Obstacle")).Length > 0;
    }
}
