using System;
using UnityEngine;
using Entities;

public class Exploring : State
{
    private Vector3? _destination;
    private Quaternion _desiredRotation;
    private Vector3 _direction;

    private readonly LayerMask _obstacleLayerMask = LayerMask.GetMask("Obstacle");

    private Animal _animal;

    private float _stopDistance = 2f;
    private float _turnSpeed = 3f;
    private float _rayDistance = 5f;

    private Type _diet;

    public Exploring(Animal animal) : base(animal.gameObject)
    {
        _animal = animal;

        if (_animal != null)
        {
            _diet = _animal.GetDiet();
        }
    }

    public override Type Tick()
    {
        _rayDistance = _animal.GetSenseRadius();
        _animal.ModifyEnergy(-_animal.GetEnergyLostPerTick());

        var nearbyPredator = NearbyPredator();
        if (nearbyPredator != null)
        {
            _animal.SetTargetPredator(nearbyPredator);
            return typeof(Fleeing);
        }

        if (_animal.IsHungry())
        {
            var foodTarget = NearbyFood();
            if (foodTarget != null)
            {
                _animal.SetTargetFood(foodTarget);
                _destination = null;
                return typeof(ChasingFood);
            }
            else
            {
                return KeepExploring();
            }
        }

        if (WantsToReproduce())
        {
            var mateTarget = NearbyMate();
            if (mateTarget != null)
            {
                if (!mateTarget.IsAdult())
                    return KeepExploring();

                if (mateTarget.GetReproductiveUrge() &&
                    mateTarget.GetState() == typeof(Exploring) || mateTarget.GetState() == typeof(GoingForMate))
                {
                    _animal.SetTargetMate(mateTarget);
                    _destination = null;
                    return typeof(GoingForMate);
                }   
            }
            else
            {
                return KeepExploring();
            }
        }

        return KeepExploring();
    }

    private Type KeepExploring()
    {
        if (_destination.HasValue == false ||
            Vector3.Distance(transform.position, _destination.Value) <= _stopDistance)
        {
            FindRandomDestination();
        }
        else
        {
            UpdateDirection();
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, _desiredRotation, _turnSpeed * Time.deltaTime);

        if (IsForwardBlocked() ||
            Vector3.Dot(transform.forward, _direction) < 0.2f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, _desiredRotation, 0.2f);
        }
        else
        {
            _animal.MoveTick(_destination.Value); // calls move method in animal
        }

        Debug.DrawRay(transform.position, _destination.Value - transform.position, Color.red);

        int k = 0;
        while (IsPathBlocked() && k < 300)
        {
            k++;
            if (k > 100)
            {
                // if stuck for too long
                Debug.Log("try to look backwards - " + transform.position);
                LookBackwards();
            }
            else
            {
                FindRandomDestination();
            }

            if (k > 280)
                Debug.LogError("too long here!! " + transform.position);
        }

        return null;
    }

    private void LookBackwards()
    {
        _destination = transform.position - 2f * transform.forward;
        UpdateDirection();
        transform.rotation = Quaternion.Lerp(transform.rotation, _desiredRotation, 0.8f);
        Debug.DrawRay(transform.position, _destination.Value - transform.position, Color.blue);
    }


    private LivingBeing NearbyFood()
    {
        Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, _animal.GetSenseRadius());

        // filter all with type _diet
        Collider[] nearbyFoodColliders = 
            Array.FindAll(nearbyColliders, c => c.gameObject.GetComponent<LivingBeing>()?.GetType() == _diet);
        LivingBeing[] nearbyFood = 
            Array.ConvertAll(nearbyFoodColliders, c => c.gameObject.GetComponent<LivingBeing>());

        // check which one's closer and target it
        LivingBeing closestFood = null;
        float smallestDist = _animal.GetSenseRadius() + 1;
        foreach (LivingBeing lb in nearbyFood)
        {
            float dist = Vector3.Distance(transform.position, lb.transform.position);
            if (dist < smallestDist && dist > Mathf.Epsilon)
            {
                if (_animal.FoodIgnored != null)
                {
                    if (_animal.FoodIgnored != lb)
                    {
                        closestFood = lb;
                        smallestDist = dist;
                    }
                }
                else
                {
                    closestFood = lb;
                    smallestDist = dist;
                }
            }
        }

        return closestFood;
    }


    private bool WantsToReproduce()
    {;
        if (_animal.IsAdult())
            return _animal.GetReproductiveUrge();
        else
            return false;
    }

    private Animal NearbyMate()
    {
        Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, _animal.GetSenseRadius());

        // filter all with same animal specie type
        Collider[] nearbySpecieColliders =
            Array.FindAll(nearbyColliders, c => c.gameObject.GetComponent<Animal>()?.GetType() == _animal.GetType());
        Animal[] potentialMates =
            Array.ConvertAll(nearbySpecieColliders, c => c.gameObject.GetComponent<Animal>());

        // check which one's closer and target it
        Animal closestMate = null;
        float smallestDist = _animal.GetSenseRadius() + 1;
        foreach (Animal a in potentialMates)
        {
            float dist = Vector3.Distance(transform.position, a.transform.position);
            if (dist < smallestDist &&
                dist > Mathf.Epsilon)
            {
                closestMate = a;
                smallestDist = dist;
            }
        }

        return closestMate;
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


    private void FindRandomDestination()
    {
        // Some hard-coded distances 
        float dist1 = 2f * transform.localScale.z; // based on the forward scale
        float dist2 = dist1 * 3f;
        Vector3 testPosition = transform.position + transform.forward * dist1
            + new Vector3(UnityEngine.Random.Range(-dist2, dist2), 1, UnityEngine.Random.Range(-dist2, dist2));

        _destination = new Vector3(testPosition.x, _animal.GroundYPos, testPosition.z);

        UpdateDirection();
    }

    private void UpdateDirection()
    {
        _direction = Vector3.Normalize(_destination.Value - transform.position);
        _direction = new Vector3(_direction.x, 0f, _direction.z);
        _desiredRotation = Quaternion.LookRotation(_direction);
    }

    private bool IsForwardBlocked()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        float dist = Vector3.Distance(_destination.Value, transform.position);
        return Physics.SphereCast(ray, 0.5f, dist, _obstacleLayerMask);
    }

    private bool IsPathBlocked()
    {
        Ray ray = new Ray(transform.position, _direction);
        float dist = Vector3.Distance(_destination.Value, transform.position);
        return Physics.SphereCast(ray, 0.5f, dist, _obstacleLayerMask);
    }
}
