using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities;

public class ChasingFood : State
{
    private Animal _animal;

    public ChasingFood(Animal animal) : base(animal.gameObject)
    {
        _animal = animal;
    }

    public override Type Tick()
    {
        Debug.Log("chasing food");
        if (Input.GetKey(KeyCode.X))
        {
            return typeof(Exploring);
        }
        else if (Input.GetKey(KeyCode.E))
        {
            return typeof(Eating);
        }
        else
        {
            return typeof(ChasingFood);
        }
    }
}
