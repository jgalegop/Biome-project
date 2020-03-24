using System;
using UnityEngine;
using Entities;

public class ChasingFood : State
{
    private Animal _animal;

    private Quaternion _targetFoodRotation;
    private Vector3 _targetFoodDirection;
    private float _turnSpeed = 3f;

    private readonly float interactionDistance = 1f;

    private float _energyLost;

    private Vector3[] _pathToTargetFood = new Vector3[0];
    private Vector3 _currentWaypoint;
    private int _targetIndex = 0;

    private readonly LayerMask _obstacleLayerMask = LayerMask.GetMask("Obstacle");

    private PathfindingDebug _debug;

    public ChasingFood(Animal animal) : base(animal.gameObject)
    {
        _animal = animal;
        _energyLost = _animal.GetEnergyLostPerTick();
        _debug = GameObject.FindObjectOfType<PathfindingDebug>();

        StateName = "Chasing food";
    }

    public override Type Tick()
    {

        _animal.ModifyEnergy(-_energyLost);

        if (_animal.TargetFood == null)
        {
            _pathToTargetFood = new Vector3[0];
            return typeof(Exploring);
        }

        // we only need to pathfind if the path is blocked, or if the target is not super close to the animal
        Vector3 vectorToTarget = _animal.TargetFood.transform.position - transform.position;

        if (IsPathBlocked(Vector3.Normalize(vectorToTarget), vectorToTarget.magnitude) &&
            Vector3.Distance(_animal.TargetFood.transform.position, transform.position) > 1.414f)
        {
            // try to pathfind
            if (_pathToTargetFood.Length == 0)
            {
                PathRequestManager.RequestPath(transform.position, _animal.TargetFood.transform.position, OnPathFound);
            }

            // waypoint correction to y-pos
            _currentWaypoint += Vector3.up * (1f - _currentWaypoint.y); // ANIMAL PLANE

            float waypointDist = Vector3.Distance(transform.position, _currentWaypoint);
            float dist = Vector3.Distance(transform.position, _animal.TargetFood.transform.position);
            if (waypointDist < interactionDistance)
            {
                _targetIndex++;
                if (_targetIndex >= _pathToTargetFood.Length)
                {
                    _targetIndex = 0;
                    _pathToTargetFood = new Vector3[0];

                    return null; 
                }
                _currentWaypoint = _pathToTargetFood[_targetIndex];
            }

            if (_debug.DebugPathfind)
            {
                DebugDrawPathLines(vectorToTarget);
            }
        }
        else
        {
            _currentWaypoint = _animal.TargetFood.transform.position;
        }


        // movement
        _targetFoodDirection = Vector3.Normalize(_currentWaypoint - transform.position);
        _targetFoodRotation = Quaternion.LookRotation(_targetFoodDirection);

        // turn faster if it is closer
        if (Vector3.Distance(transform.position, _currentWaypoint) < 2 * interactionDistance)
            transform.rotation = Quaternion.Lerp(transform.rotation, _targetFoodRotation, 4 * _turnSpeed * Time.deltaTime);
        else
            transform.rotation = Quaternion.Lerp(transform.rotation, _targetFoodRotation, _turnSpeed * Time.deltaTime);

        _animal.MoveTick(_currentWaypoint);

        float newDist = Vector3.Distance(transform.position, _animal.TargetFood.transform.position);
        if (newDist < interactionDistance)
        {
            _pathToTargetFood = new Vector3[0];
            return typeof(Eating);
        }

        return null;
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            _pathToTargetFood = newPath;
            _currentWaypoint = _pathToTargetFood[0];
        }
    }

    private bool IsPathBlocked(Vector3 dir, float dist)
    {
        Ray ray = new Ray(transform.position, dir);
        return Physics.SphereCast(ray, 0.5f, dist, _obstacleLayerMask);
    }



    private void DebugDrawPathLines(Vector3 vectorToTarget)
    {
        Debug.DrawRay(transform.position, vectorToTarget, Color.white);
        Debug.LogError("Pathfind test " + _animal.gameObject.name);

        for (int i = _targetIndex; i < _pathToTargetFood.Length; i++)
        {
            if (i == _targetIndex)
            {
                Debug.DrawLine(transform.position, _pathToTargetFood[i], Color.black);
            }
            else
            {
                Debug.DrawLine(_pathToTargetFood[i - 1], _pathToTargetFood[i], Color.black);
            }
        }
    }
}
