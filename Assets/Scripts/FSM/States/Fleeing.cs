using System;
using UnityEngine;
using Entities;

public class Fleeing : State
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

    public Fleeing(Animal animal) : base(animal.gameObject)
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

        if (_animal.TargetPredator != null)
        {
            if (PredatorInsideRadius())
                return KeepFleeing(); 
        }

        return typeof(Exploring);
    }

    private Type KeepFleeing()
    {
        if (_destination.HasValue == false ||
            Vector3.Distance(transform.position, _destination.Value) <= _stopDistance)
        {
            FindRandomFleeingDestination();
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
            _animal.MoveTick(_destination.Value, 1.5f); // calls move method in animal with a speed modifier
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
                FindRandomFleeingDestination();
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

    private bool PredatorInsideRadius()
    {
        float predatorDist = Vector3.Distance(_animal.TargetPredator.transform.position, transform.position);
        bool inDist = false;
        if (predatorDist < _animal.GetSenseRadius())
            inDist = true;
        else
            _animal.SetTargetPredator(null);
        return inDist;
    }


    private void FindRandomFleeingDestination()
    {
        // Some hard-coded distances 
        float dist1 = 3f * transform.localScale.z; // based on the forward scale
        float dist2 = dist1 * 1f;
        Vector3 fleeForwardDir = Vector3.Normalize(transform.position - _animal.TargetPredator.transform.position);
        Vector3 testPosition = transform.position + fleeForwardDir * dist1
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
