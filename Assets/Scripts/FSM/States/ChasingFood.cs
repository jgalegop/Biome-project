using System;
using UnityEngine;
using Entities;

public class ChasingFood : State
{
    private Animal _animal;
    private float _moveSpeed;

    private Quaternion _targetFoodRotation;
    private Vector3 _targetFoodDirection;
    private float _turnSpeed = 3f;

    private readonly float interactionDistance = 1f;

    private float _energyLost = 5f;

    public ChasingFood(Animal animal) : base(animal.gameObject)
    {
        _animal = animal;
        _moveSpeed = _animal.GetMoveSpeed();
    }

    public override Type Tick()
    {
        _animal.ModifyEnergy(-_energyLost);

        // TODO: take obstacles into account (some pathfinding)

        if (_animal.TargetFood == null)
        {
            return typeof(Exploring);
        }

        _targetFoodDirection = Vector3.Normalize(_animal.TargetFood.transform.position - transform.position);
        _targetFoodRotation = Quaternion.LookRotation(_targetFoodDirection);

        transform.rotation = Quaternion.Lerp(transform.rotation, _targetFoodRotation, _turnSpeed * Time.deltaTime);
        transform.Translate(Vector3.forward * _moveSpeed * Time.deltaTime);

        float dist = Vector3.Distance(transform.position, _animal.TargetFood.transform.position);
        if (dist < interactionDistance)
        {
            return typeof(Eating);
        }

        return null;
    }
}
