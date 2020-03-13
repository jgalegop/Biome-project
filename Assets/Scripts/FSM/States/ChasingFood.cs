using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities;

public class ChasingFood : State
{
    private Animal _animal;
    private float _moveSpeed;

    private readonly float interactionDistance = 1f;

    public ChasingFood(Animal animal) : base(animal.gameObject)
    {
        _animal = animal;
        _moveSpeed = _animal.GetMoveSpeed();
    }

    public override Type Tick()
    {
        // TODO: take obstacles into account (some pathfinding)
        Debug.Log("chasing");
        if (_animal.TargetFood == null)
        {
            return typeof(Exploring);
        }
        transform.LookAt(_animal.TargetFood.transform.position);
        transform.Translate(Vector3.forward * _moveSpeed * Time.deltaTime);

        float dist = Vector3.Distance(transform.position, _animal.TargetFood.transform.position);
        if (dist < interactionDistance)
        {
            return typeof(Eating);
        }

        return null;
    }
}
