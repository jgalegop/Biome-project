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
        private readonly float _senseRadius = 8f;

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
            moveSpeed = _moveSpeed + Random.Range(-1f, 1f);
            senseRadius = _senseRadius + Random.Range(-3f, 3f);
            base.Awake();
        }

        public override void MoveTick(Vector3 destination)
        {
            _rabbitMovement.Tick(destination);
        }


        // debug
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, _senseRadius);
        }
    }
}

