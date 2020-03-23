using UnityEngine;
using System;
using Random = UnityEngine.Random;

namespace Entities.Animals
{
    [RequireComponent(typeof(RabbitMovement))]
    public class Rabbit : Animal
    {
        // traits
        private readonly float _moveSpeed = 2.5f;
        private readonly float _senseRadius = 10f;

        private readonly float _moveSpeedVar = 1f;
        private readonly float _senseRadiusVar = 3f;

        // diet
        private readonly Type _diet = typeof(Plant);

        // energy
        private float _maxEnergy = 100;

        // movement
        private RabbitMovement _rabbitMovement => GetComponent<RabbitMovement>();


        // class constructor
        public Rabbit()
        {
            // constants since construction
            diet = _diet;

            // variable
            MaxEnergy = _maxEnergy;
        }


        // called in Awake so each object with this class has different initial values
        public override void Awake()
        {
            moveSpeed = _moveSpeed + Random.Range(-_moveSpeedVar, _moveSpeedVar);
            senseRadius = _senseRadius + Random.Range(-_senseRadiusVar, _senseRadiusVar);

            // TO DO: redo the energy loss by move speed
            float clamp = Clamp(_moveSpeed - 0.5f * _moveSpeedVar, _moveSpeed + 0.5f * _moveSpeedVar, moveSpeed);
            energyLost = _moveSpeed + 0.5f * _moveSpeedVar * (2f * clamp - 1f);
            base.Awake();
        }

        public override void MoveTick(Vector3 destination)
        {
            _rabbitMovement.Tick(destination);
        }

        // utility
        private float Clamp(float a, float b, float x)
        {
            return Mathf.Clamp01((x - a) / (b - a));
        }


        // debug
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, _senseRadius);
        }
    }
}

