using System;
using Entities;
using UnityEngine;

public class Mating : State
{
    private float _energyCost = 5f;
    private float _frickTime = 0f;
    private float _danceSpeed = 1.5f;
    private float _moveSpeed;
    private bool _startedDance = false;
    private Vector3 _centerPosition;
    private Quaternion _centerRotation;

    private Animal _animal;
    public Mating(Animal animal) : base(animal.gameObject)
    {
        _animal = animal;
        _moveSpeed = _animal.GetMoveSpeed();
    }

    public override Type Tick()
    {
        _animal.ModifyEnergy(-_energyCost);
        Debug.Log("MATING TIME");

        DanceTick();

        if (_frickTime > 5f)
        {
            _animal.StopReproductiveUrge();
            if (!_animal.TargetMate.GetReproductiveUrge())
            {
                _startedDance = false;
                return typeof(Exploring);
            }
        }

        _frickTime += Time.deltaTime;
        return null;
    }

    private void DanceTick()
    {
        if (!_startedDance)
        {
            _startedDance = true;
            _centerPosition = 0.5f * (transform.position + _animal.TargetMate.transform.position);
            transform.rotation = Quaternion.LookRotation(transform.right);
        }
        else
        {
            _centerRotation = Quaternion.LookRotation(_centerPosition - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, _centerRotation, _danceSpeed * Time.deltaTime);
        }

        transform.Translate(2f * Vector3.forward * _danceSpeed * Time.deltaTime);
        transform.position += Vector3.up * (1f - transform.position.y); 
    }
}
