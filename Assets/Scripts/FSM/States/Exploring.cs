using System;
using UnityEngine;
using Entities;

public class Exploring : State
{
    private Vector3? _destination;
    private Quaternion _desiredRotation;
    private Vector3 _direction;

    private readonly LayerMask _obstacleLayerMask = LayerMask.GetMask("Obstacle", "Animal");

    private Animal _animal;

    private float _stopDistance = 2f;
    private float _turnSpeed = 3f;
    private float _rayDistance = 5f;

    private float _senseRadius;

    private Type _diet;

    private float _energyLost;
    private float _hungerThreshold = 50f;

    public Exploring(Animal animal) : base(animal.gameObject)
    {
        _animal = animal;

        if (_animal != null)
        {
            _senseRadius = _animal.GetSenseRadius();
            _diet = _animal.GetDiet();

            _energyLost = _animal.GetEnergyLostPerTick();
        }
    }

    public override Type Tick()
    {
        _animal.ModifyEnergy(-_energyLost);

        if (IsHungry())
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
                if (mateTarget.GetReproductiveUrge() &&
                    mateTarget.GetState() == typeof(Exploring) || mateTarget.GetState() == typeof(GoingForMate))
                {
                    Debug.Log(gameObject.name + " going for mate " + mateTarget.gameObject.name);
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

        // turn towards target destination
        float turnSpeedMod = 1f; // 2 * (1.2f - Vector3.Dot(transform.forward, _direction));
        transform.rotation = Quaternion.Slerp(transform.rotation, _desiredRotation, turnSpeedMod * _turnSpeed * Time.deltaTime);

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

        while (IsPathBlocked())
        {
            FindRandomDestination();
        }

        return null;
    }

    private bool IsHungry()
    {
        return _animal.GetEnergy() < _hungerThreshold;
    }

    private LivingBeing NearbyFood()
    {
        Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, _senseRadius);

        // filter all with type _diet
        Collider[] nearbyFoodColliders = 
            Array.FindAll(nearbyColliders, c => c.gameObject.GetComponent<LivingBeing>()?.GetType() == _diet);
        LivingBeing[] nearbyFood = 
            Array.ConvertAll(nearbyFoodColliders, c => c.gameObject.GetComponent<LivingBeing>());

        // check which one's closer and target it
        LivingBeing closestFood = null;
        float smallestDist = _senseRadius + 1;
        foreach (LivingBeing lb in nearbyFood)
        {
            float dist = Vector3.Distance(transform.position, lb.transform.position);
            if (dist < smallestDist &&
                dist > Mathf.Epsilon)
            {
                closestFood = lb;
                smallestDist = dist;
            }
        }

        return closestFood;
    }


    private bool WantsToReproduce()
    {
        return _animal.GetReproductiveUrge();
    }

    private Animal NearbyMate()
    {
        Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, _senseRadius);

        // filter all with same animal specie type
        Collider[] nearbySpecieColliders =
            Array.FindAll(nearbyColliders, c => c.gameObject.GetComponent<Animal>()?.GetType() == _animal.GetType());
        Animal[] potentialMates =
            Array.ConvertAll(nearbySpecieColliders, c => c.gameObject.GetComponent<Animal>());

        // check which one's closer and target it
        Animal closestMate = null;
        float smallestDist = _senseRadius + 1;
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


    private void FindRandomDestination()
    {
        // Some hard-coded distances 
        float dist1 = 2f * transform.localScale.z; // based on the forward scale
        float dist2 = dist1 * 3f;
        Vector3 testPosition = transform.position + transform.forward * dist1
            + new Vector3(UnityEngine.Random.Range(-dist2, dist2), 1, UnityEngine.Random.Range(-dist2, dist2));

        _destination = new Vector3(testPosition.x, 1f, testPosition.z);

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
        return Physics.SphereCast(ray, 0.5f, _rayDistance, _obstacleLayerMask);
    }

    private bool IsPathBlocked()
    {
        Ray ray = new Ray(transform.position, _direction);
        return Physics.SphereCast(ray, 0.5f, _rayDistance, _obstacleLayerMask);
    }
}
