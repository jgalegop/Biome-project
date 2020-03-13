using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities;
using System.Linq;

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

    private float _moveSpeed;
    private float _senseRadius;

    private Type _diet;

    public Exploring(Animal animal) : base(animal.gameObject)
    {
        _animal = animal;

        if (_animal != null)
        {
            _moveSpeed = _animal.GetMoveSpeed();
            _senseRadius = _animal.GetSenseRadius();
            _diet = _animal.GetDiet();
        }
    }

    public override Type Tick()
    {
        // Debug.Log("exploring");
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

        return KeepExploring();
    }

    private Type KeepExploring()
    {
        if (_destination.HasValue == false ||
                            Vector3.Distance(transform.position, _destination.Value) <= _stopDistance)
        {
            FindRandomDestination();
        }

        // turn towards target destination
        transform.rotation = Quaternion.Slerp(transform.rotation, _desiredRotation, _turnSpeed * Time.deltaTime);

        if (IsForwardBlocked())
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, _desiredRotation, 0.2f);
        }
        else
        {
            // moves facing forward
            transform.Translate(Vector3.forward * _moveSpeed * Time.deltaTime);
        }

        Debug.DrawRay(transform.position, _direction * _rayDistance, Color.red);
        

        while (IsPathBlocked())
        {
            FindRandomDestination();
        }

        Debug.DrawRay(transform.position, _destination.Value - transform.position, Color.magenta);

        return null;
    }

    private bool IsHungry()
    {
        return Input.GetKeyDown(KeyCode.H);
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

    private void FindRandomDestination()
    {

        // Some hard-coded distances 
        float dist1 = 2f * transform.localScale.z; // based on the forward scale
        float dist2 = dist1 * 3f;
        Vector3 testPosition = (transform.position + (transform.forward * dist1)
            + new Vector3(UnityEngine.Random.Range(-dist2, dist2), 0, UnityEngine.Random.Range(-dist2, dist2)));

        _destination = new Vector3(testPosition.x, 1f, testPosition.z);

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
