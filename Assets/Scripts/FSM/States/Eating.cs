using System;
using Entities;
using UnityEngine;

public class Eating : State
{
    private float _energyGained = 25f;

    private Animal _animal;
    public Eating(Animal animal) : base(animal.gameObject)
    {
        _animal = animal;
    }

    public override Type Tick()
    {
        if (_animal.DebugModeOn)
            Debug.Log("eating");

        _animal.ModifyEnergy(+_energyGained);

        if (_animal.TargetFood == null)
        {
            return typeof(Exploring);
        }

        _animal.TargetFood.BeingEaten();
        return null;
    }
}
