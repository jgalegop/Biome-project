﻿using UnityEngine;
using System.Collections.Generic;

public class SpawnButtons : MonoBehaviour
{
    [SerializeField]
    private int _spawnNumber = 1;

    private PathfindGrid _grid = null;

    private int _findPosTries = 10;
    private Vector3 _newWorldPos = Vector3.zero;

    private List<GameObject> _spawnedPrefabs = new List<GameObject>();
    private List<GameObject> _spawnedPrefabParents = new List<GameObject>();

    private void Awake()
    {
        _grid = FindObjectOfType<PathfindGrid>();
        if (_grid == null)
            Debug.LogError("scene needs a pathfindgrid object");
    }

    public void SpawnPrefab(GameObject prefab)
    {
        GameObject parent;
        if (!PrefabHasBeenSpawnedBefore(prefab))
        {
            parent = NewPrefabparent(prefab);
        }
        else
        {
            parent = _spawnedPrefabParents.Find((x) => x.name == prefab.name + "s");
        }

        if (parent == null)
            Debug.LogError("Parent still null");

        int k = 0;
        while (k < _spawnNumber)
        {
            if (SuitablePositionFound())
            {
                GameObject go = Instantiate(prefab, _newWorldPos, Quaternion.identity);
                go.transform.SetParent(parent.transform);
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

    private bool PrefabHasBeenSpawnedBefore(GameObject prefab)
    {
        if (_spawnedPrefabs.Contains(prefab))
        {
            return true;
        }
        else
        {
            _spawnedPrefabs.Add(prefab);
            return false;
        }
    }

    private GameObject NewPrefabparent(GameObject prefab)
    {
        GameObject parent = new GameObject(prefab.name + "s");
        _spawnedPrefabParents.Add(parent);
        return parent;
    }

    public void SetSpawnNumber(int number)
    {
        _spawnNumber = number;
    }
}