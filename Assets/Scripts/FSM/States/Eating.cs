using System;
using Entities;
using UnityEngine;

public class Eating : State
{
    private float _energyGained = 50f;

    private Animal _animal;
    public Eating(Animal animal) : base(animal.gameObject)
    {
        _animal = animal;
    }

    public override Type Tick()
    {
        if (_animal.TargetFood == null)
        {
            return typeof(Exploring);
        }

        var nearbyPredator = NearbyPredator();
        if (nearbyPredator != null)
        {
            _animal.SetTargetPredator(nearbyPredator);
            return typeof(Fleeing);
        }

        if (_animal.TargetFood.GetComponent<Animal>() == null)
            _animal.ModifyEnergy(+_energyGained);
        else
            _animal.ModifyEnergy(+ 3 *_energyGained);

        _animal.ModifyEnergy(-_animal.GetEnergyLostPerTick());

        _animal.TargetFood.BeingEaten();
        return null;
    }

    private Animal NearbyPredator()
    {
        Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, _animal.GetSenseRadius());

        // filter all with same diet as this animal specie type
        Collider[] nearbySpecieColliders =
            Array.FindAll(nearbyColliders, c => c.gameObject.GetComponent<Animal>()?.GetDiet() == _animal.GetType());
        Animal[] potentialPredators =
            Array.ConvertAll(nearbySpecieColliders, c => c.gameObject.GetComponent<Animal>());

        // check which ones are chasing
        Animal closestPredator = null;
        float smallestDist = _animal.GetSenseRadius() + 1;
        foreach (Animal a in potentialPredators)
        {
            // if predator is chasing this animal
            if (a.GetState() == typeof(ChasingFood))
            {
                if (a != null && a.TargetFood != null)
                {
                    if (a.TargetFood.gameObject == _animal.gameObject)
                    {
                        float dist = Vector3.Distance(transform.position, a.transform.position);
                        if (dist < smallestDist &&
                            dist > Mathf.Epsilon)
                        {
                            closestPredator = a;
                            smallestDist = dist;
                        }
                    }
                }
            }
        }

        return closestPredator;
    }
}
