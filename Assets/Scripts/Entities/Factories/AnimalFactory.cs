using Entities;
using UnityEngine;
/// <summary>
/// Factory for prefabs of Animals
/// </summary>
[RequireComponent(typeof(Animal))]
public class AnimalFactory : MonoBehaviour
{
    // Reference to prefab of whatever type.
    private GameObject _gameObjectCopy;
    /// <summary>
    /// Creating new instance of prefab.
    /// </summary>
    /// <returns>New instance of prefab.</returns>
    public GameObject GetNewInstance()
    {
        return Instantiate(_gameObjectCopy);
        // later it takes from pool
    }

    public void SetInstance(GameObject go)
    {
        _gameObjectCopy = go;
        // later sets it to pool
    }
}