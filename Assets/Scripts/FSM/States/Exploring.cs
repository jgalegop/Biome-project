using System;
using System.Collections;
using System.Collections.Generic;
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
        // sensed food
        if (IsFoodNearby())
        {
            return typeof(ChasingFood);
        }
        // eating food
        else if (Input.GetKey(KeyCode.E))
        {
            return typeof(Eating);
        }
        else
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

            return null;
        }
    }

    private bool IsFoodNearby()
    {
        Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, _senseRadius);

        return false;
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
