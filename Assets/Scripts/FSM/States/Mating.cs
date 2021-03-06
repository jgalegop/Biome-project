﻿using System;
using Entities;
using UnityEngine;
using DG.Tweening;

public class Mating : State
{
    private float _energyCost = 5f;

    private float _jumpPower = 1f;
    private readonly float _jumpDuration = 0.5f;
    private bool _isJumping = false;
    private Vector3 _groundScaleFactor = new Vector3(1.2f, 0.8f, 1.2f);
    private Vector3 _airScaleFactor = new Vector3(0.8f, 1.2f, 0.8f);
    private Vector3 _groundScale;
    private Vector3 _airScale;
    private float _restTime = 0.25f;
    private readonly int _maxNumberOfJumps = 3;
    private int _numberOfJumps = 0;

    private Animal _animal;
    public Mating(Animal animal) : base(animal.gameObject)
    {
        _animal = animal;
        _energyCost = _animal.GetEnergyLostPerTick();
    }

    public override Type Tick()
    {
        _animal.ModifyEnergy(-_energyCost);

        var nearbyPredator = NearbyPredator();
        if (nearbyPredator != null)
        {
            _animal.SetTargetPredator(nearbyPredator);
            return typeof(Fleeing);
        }

        if (_animal.TargetMate == null ||
            !_animal.TargetMate.IsAdult())
        {
            return typeof(Exploring);
        }
            

        if (_numberOfJumps >= _maxNumberOfJumps)
        {
            _animal.StopReproductiveUrge();
            if (!_animal.TargetMate.GetReproductiveUrge())
            {
                _animal.SpawnOffspring();
            }
            _numberOfJumps = 0;
            return typeof(Exploring);
        }

        DanceTick();

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

    private void DanceTick()
    {
        FaceMate();

        if (_isJumping)
            return;

        SetScales();

        // variation in jump power
        _jumpPower += UnityEngine.Random.Range(-0.2f, 0.3f);

        Jump();
    }

    private void SetScales()
    {
        _groundScale = Vector3.Scale(_animal.AdultScale, _groundScaleFactor);
        _airScale = Vector3.Scale(_animal.AdultScale, _airScaleFactor);
    }

    private void FaceMate()
    {
        transform.rotation = Quaternion.LookRotation(_animal.TargetMate.transform.position - transform.position);
    }

    private void Jump()
    {
        _isJumping = true;
        // scale change - preparing for jump
        transform.DOScale(_groundScale, 0.5f * _jumpDuration)
                 .OnComplete(MakeJump);

        // correction to y-coord so it sits on the ground
        float _correctionY = _animal.AdultScale.y - 0.5f * (_animal.AdultScale.y - _groundScaleFactor.y);
        transform.DOLocalMoveY(_correctionY, 0.5f * _jumpDuration);
    }

    private void MakeJump()
    {
        // start jump with scale at roughly half jump
        // make sure that jumps into the same plane
        transform.DOJump(transform.position, _jumpPower, 1, _jumpDuration);
        transform.DOScale(_airScale, 0.7f * _jumpDuration)
                 .OnComplete(LandJump);
    }

    private void LandJump()
    {
        // landing
        transform.DOScale(_groundScale, 0.3f * _jumpDuration)
                 .OnComplete(Recover);
    }

    private void Recover()
    {
        // recover original configuration
        transform.DOScale(_animal.AdultScale, 0.1f)
                 .OnComplete(NotOnAir);
        // correction to y-coord
        transform.DOLocalMoveY(_animal.AdultScale.y, 0.1f);
    }

    private void NotOnAir()
    {
        // virtually does nothing (rest time)
        transform.DOScale(_animal.AdultScale, _restTime).OnComplete(ResetIsJumping);
    }

    private void ResetIsJumping()
    {
        _isJumping = false;
        _numberOfJumps++;
    }
}