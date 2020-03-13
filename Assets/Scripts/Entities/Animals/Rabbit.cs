using UnityEngine;
using System;

namespace Entities.Animals
{
    public class Rabbit : Animal
    {
        // traits
        private readonly float _moveSpeed = 2.5f;
        private readonly float _senseRadius = 8f;

        // diet
        private readonly Type _diet = typeof(Plant);

        // energy
        private float _maxEnergy = 100;

        // constructor
        public Rabbit()
        {
            // constants
            moveSpeed = _moveSpeed;
            senseRadius = _senseRadius;
            diet = _diet;

            // variable
            maxEnergy = _maxEnergy;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, _senseRadius);
        }
    }
}

