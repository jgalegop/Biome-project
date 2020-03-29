using UnityEngine;
/// <summary>
/// Factory design pattern with generic twist!
/// </summary>
public class GenericFactory<T> : MonoBehaviour where T : MonoBehaviour
{
    // Reference to prefab of whatever type.
    private T gameObjectCopy;
    /// <summary>
    /// Creating new instance of prefab.
    /// </summary>
    /// <returns>New instance of prefab.</returns>
    public T GetNewInstance()
    {
        return Instantiate(gameObjectCopy);
        // later it takes from pool
    }

    // THIS SHOULD SPAWN OBJECT COPIES OF ITSELF, WHICH MAY BE MODIFIED AT SPAWN
    /*
    public void SetNewInstance(T)
    {
        gameObjectCopy = object;
        // later it takes from pool
    }
    */
}