using UnityEngine;
using System;
using Random = UnityEngine.Random;

namespace Entities.Animals
{
    public class Rabbit : Animal
    {
        // traits
        private float _moveSpeed = 2.5f;
        private float _senseRadius = 10f;

        private readonly float _moveSpeedVar = 1f;
        private readonly float _senseRadiusVar = 3f;

        // diet
        private readonly Type _diet = typeof(Plant);

        // energy
        private float _maxEnergy = 100;

        // movement
        private RabbitMovement _rabbitMovement;


        // class constructor
        public Rabbit(float moveSpeed, float senseRadius)
        {
            _moveSpeed = moveSpeed;
            _senseRadius = senseRadius;

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
            base.Awake();

            _rabbitMovement = GetComponent<RabbitMovement>();
        }

        public override void MoveTick(Vector3 destination)
        {
            if (_rabbitMovement != null)
                _rabbitMovement.Tick(destination); 
            else
                base.MoveTick(destination);
        }


        // debug
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, _senseRadius);
        }
    }
}

