﻿using Entities;
using System;
using UnityEngine;

public class StatisticsManager : MonoBehaviour
{
    public static event Action<Animal> OnAnimalNumberIncreased = delegate { };
    public static event Action<Animal> OnAnimalNumberDecreased = delegate { };

    public static void AnimalIsBorn(Animal animal)
    {
        OnAnimalNumberIncreased?.Invoke(animal);
    }

    public static void AnimalHasDied(Animal animal)
    {
        OnAnimalNumberDecreased?.Invoke(animal);
    }
}
