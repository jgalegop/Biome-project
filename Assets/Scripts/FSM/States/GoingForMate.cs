using System;
using System.Collections.Generic;
using UnityEngine;
using Entities;

public class GoingForMate : State
{
    private Animal _animal;

    private Quaternion _targetMateRotation;
    private Vector3 _targetMateDirection;
    private float _turnSpeed = 3f;

    private readonly float interactionDistance = 2f;

    private float _energyLost;

    private Vector3[] _pathToTargetMate = new Vector3[0];
    private Vector3 _currentWaypoint;
    private int _targetIndex = 0;
    private Vector3 _vectorToTarget;
    private List<Type> _availableStates;

    private readonly LayerMask _obstacleLayerMask = LayerMask.GetMask("Obstacle");

    private PathfindingDebug _debug;

    public GoingForMate(Animal animal) : base(animal.gameObject)
    {
        _animal = animal;
        _energyLost = _animal.GetEnergyLostPerTick();
        _debug = GameObject.FindObjectOfType<PathfindingDebug>();

        StateName = "Going for mate";

        _availableStates = new List<Type> { typeof(Exploring), typeof(GoingForMate), typeof(Mating) };
    }

    public override Type Tick()
    {
        _animal.ModifyEnergy(-_energyLost);

        if (TargetNotSuitable())
        {
            _pathToTargetMate = new Vector3[0];
            return typeof(Exploring);
        }

        return FindingMate();
    }

    private bool TargetNotSuitable()
    {
        bool targetInexistent = _animal.TargetMate == null;
        bool targetAvailable = _availableStates.Contains(_animal.TargetMate?.GetState());
        bool targetHasNoTarget = _animal.TargetMate.TargetMate == null;
        bool targetMatingWithOther = _animal.TargetMate?.GetState() == typeof(Mating) && 
                                     _animal.TargetMate?.TargetMate.gameObject != gameObject;

        return targetInexistent || !targetAvailable || targetHasNoTarget || targetMatingWithOther;
    }

    private Type FindingMate()
    {
        if (NeedsPathfinding())
        {
            // needs new path to target
            if (_pathToTargetMate.Length == 0)
            {
                PathRequestManager.RequestPath(transform.position, _animal.TargetMate.transform.position, OnPathFound);
            }

            // waypoint correction to y-pos
            _currentWaypoint += Vector3.up * (1f - _currentWaypoint.y); // ANIMAL PLANE

            float waypointDist = Vector3.Distance(transform.position, _currentWaypoint);
            float dist = Vector3.Distance(transform.position, _animal.TargetMate.transform.position);

            // close to end
            if (waypointDist < interactionDistance)
            {
                _targetIndex++;
                if (_targetIndex >= _pathToTargetMate.Length)
                {
                    // repeats and resets path
                    _targetIndex = 0;
                    _pathToTargetMate = new Vector3[0];

                    return null;
                }
                _currentWaypoint = _pathToTargetMate[_targetIndex];
            }

            if (_debug.DebugPathfind)
                DebugDrawPathLines(_vectorToTarget);
        }
        else
        {
            // in direction of target mate, but slightly tilted towards current position, so that it doesn't jump
            _currentWaypoint = (0.25f * _animal.TargetMate.transform.position + 0.75f * transform.position);
            // waypoint correction to y-pos
            _currentWaypoint += Vector3.up * (1f - _currentWaypoint.y); // ANIMAL PLANE
        }

        // movement
        SetUpRotations();
        _animal.MoveTick(_currentWaypoint);

        float newDist = Vector3.Distance(transform.position, _animal.TargetMate.transform.position);
        if (newDist < interactionDistance &&
            _animal.TargetMate.TargetMate?.gameObject == _animal.gameObject)
        {
            _pathToTargetMate = new Vector3[0];
            return typeof(Mating);
        }

        return null;
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            _pathToTargetMate = newPath;
            _currentWaypoint = _pathToTargetMate[0];
        }
    }

    private bool IsPathBlocked(Vector3 dir, float dist)
    {
        Ray ray = new Ray(transform.position, dir);
        return Physics.SphereCast(ray, 0.5f, dist, _obstacleLayerMask);
    }

    private bool NeedsPathfinding()
    {
        // we only need to pathfind if the path is blocked, or if the target is not super close to the animal
        _vectorToTarget = _animal.TargetMate.transform.position - transform.position;
        bool pathBlocked = IsPathBlocked(Vector3.Normalize(_vectorToTarget), _vectorToTarget.magnitude);
        bool tooCloseToTarget = _vectorToTarget.magnitude < 1.414f;
        return pathBlocked && !tooCloseToTarget;
    }

    private void SetUpRotations()
    {
        // movement
        _targetMateDirection = Vector3.Normalize(_currentWaypoint - transform.position);
        _targetMateRotation = Quaternion.LookRotation(_targetMateDirection);

        // turn faster if it is closer
        if (Vector3.Distance(transform.position, _currentWaypoint) < 2 * interactionDistance)
            transform.rotation = Quaternion.Lerp(transform.rotation, _targetMateRotation, 4 * _turnSpeed * Time.deltaTime);
        else
            transform.rotation = Quaternion.Lerp(transform.rotation, _targetMateRotation, _turnSpeed * Time.deltaTime);
    }



    private void DebugDrawPathLines(Vector3 vectorToTarget)
    {
        Debug.DrawRay(transform.position, vectorToTarget, Color.white);
        Debug.LogError("Pathfind test " + _animal.gameObject.name);

        for (int i = _targetIndex; i < _pathToTargetMate.Length; i++)
        {
            if (i == _targetIndex)
            {
                Debug.DrawLine(transform.position, _pathToTargetMate[i], Color.black);
            }
            else
            {
                Debug.DrawLine(_pathToTargetMate[i - 1], _pathToTargetMate[i], Color.black);
            }
        }
    }
}

