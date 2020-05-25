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
    private Vector3 _vectorToTarget;

    private int pathFailures = 0;

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

        var nearbyPredator = NearbyPredator();
        if (nearbyPredator != null)
        {
            _animal.SetTargetPredator(nearbyPredator);
            return typeof(Fleeing);
        }

        if (_animal.TargetFood == null || _animal.TargetFood == _animal.FoodIgnored)
        {
            _pathToTargetFood = new Vector3[0];
            return typeof(Exploring);
        }

        return FindingFood();
    }

    private Type FindingFood()
    {
        if (NeedsPathfinding())
        {
            // needs new path to target
            if (_pathToTargetFood.Length == 0)
            {
                PathRequestManager.RequestPath(transform.position, _animal.TargetFood.transform.position, OnPathFound);
            }

            // waypoint correction to y-pos
            _currentWaypoint += Vector3.up * (1f - _currentWaypoint.y); // ANIMAL PLANE

            float waypointDist = Vector3.Distance(transform.position, _currentWaypoint);
            float dist = Vector3.Distance(transform.position, _animal.TargetFood.transform.position);

            // close to end
            if (waypointDist < interactionDistance)
            {
                _targetIndex++;
                if (_targetIndex >= _pathToTargetFood.Length && _pathToTargetFood.Length > 0)
                {
                    // repeats and resets path
                    _targetIndex = 0;
                    _pathToTargetFood = new Vector3[0];
                    return null;
                }

                if (_pathToTargetFood.Length == 0)
                {
                    _currentWaypoint = _animal.TargetFood.transform.position;
                    // waypoint correction to y-pos
                    _currentWaypoint += Vector3.up * (1f - _currentWaypoint.y); // ANIMAL PLANE
                }
                else
                {
                    _currentWaypoint = _pathToTargetFood[_targetIndex];
                }
            }

            if (_debug.DebugPathfind)
                DebugDrawPathLines(_vectorToTarget);
        }
        else
        {
            _currentWaypoint = _animal.TargetFood.transform.position;
            // waypoint correction to y-pos
            _currentWaypoint += Vector3.up * (_animal.GroundYPos + 0.5f * _animal.transform.localScale.y - _currentWaypoint.y); // ANIMAL PLANE
        }

        // movement
        SetUpRotations();
        if (_animal.TargetFood.GetComponent<Animal>() == null)
            _animal.MoveTick(_currentWaypoint);
        else
            _animal.MoveTick(_currentWaypoint, 1.5f);

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
            pathFailures = 0;
            _animal.UnsucsesfulPathfinds = 0;
        }
        else
        {
            pathFailures++;
            _animal.UnsucsesfulPathfinds++;
            // if we have too many path failures, then we should ignore that target food, and keep exploring
            if (pathFailures > 50)
            {
                _animal.SetFoodIgnored(_animal.TargetFood);
                _animal.SetTargetFood(null);
            }   
        }
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

    private bool IsPathBlocked(Vector3 dir, float dist)
    {
        Ray ray = new Ray(transform.position, dir);
        Debug.DrawRay(transform.position, _animal.TargetFood.transform.position - transform.position, Color.blue);
        Debug.DrawRay(transform.position, _currentWaypoint - transform.position, Color.magenta);
        return Physics.SphereCast(ray, 0.5f, dist, _obstacleLayerMask);
    }

    private bool NeedsPathfinding()
    {
        // we only need to pathfind if the path is blocked, or if the target is not super close to the animal
        _vectorToTarget = _animal.TargetFood.transform.position - transform.position;
        bool pathBlocked = IsPathBlocked(Vector3.Normalize(_vectorToTarget), _vectorToTarget.magnitude);
        bool tooCloseToTarget = Vector3.Distance(_animal.TargetFood.transform.position, transform.position) < 1.414f;
        return pathBlocked && !tooCloseToTarget;   
    }

    private void SetUpRotations()
    {
        // movement
        _targetFoodDirection = Vector3.Normalize(_currentWaypoint - transform.position);
        _targetFoodRotation = Quaternion.LookRotation(_targetFoodDirection);

        // turn faster if it is closer
        if (Vector3.Distance(transform.position, _currentWaypoint) < 2 * interactionDistance)
            transform.rotation = Quaternion.Lerp(transform.rotation, _targetFoodRotation, 4 * _turnSpeed * Time.deltaTime);
        else
            transform.rotation = Quaternion.Lerp(transform.rotation, _targetFoodRotation, _turnSpeed * Time.deltaTime);
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
