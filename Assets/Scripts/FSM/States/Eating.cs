using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities;

public class Eating : State
{
    private Animal _animal;
    public Eating(Animal animal) : base(animal.gameObject)
    {
        _animal = animal;
    }

    public override Type Tick()
    {
        Debug.Log("eating");
        if (_animal.TargetFood == null)
        {
            return typeof(Exploring);
        }

        _animal.TargetFood.BeingEaten();
        return null;
    }
}
