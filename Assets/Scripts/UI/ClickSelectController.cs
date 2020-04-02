using System;
using UnityEngine;
using Entities;

public class ClickSelectController : MonoBehaviour
{
    [SerializeField]
    private Camera _camera = null;

    private int _obstacleLayerMask;

    public static event Action<Animal> OnSelectedAnimalChanged = delegate { };

    public static Animal SelectedAnimal { get; private set; }

    private void Awake()
    {
        _obstacleLayerMask = 1 << LayerMask.NameToLayer("Obstacle");
        _obstacleLayerMask = ~_obstacleLayerMask;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.red, 1f); 
            if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, _obstacleLayerMask))
            {
                var animal = hitInfo.collider.GetComponent<Animal>();
                SelectedAnimal = animal;
                OnSelectedAnimalChanged?.Invoke(animal);
            }
            else
            {
                SelectedAnimal = null;
                OnSelectedAnimalChanged?.Invoke(null);
            }
        }
    }
}
