using System.Collections;
using System.Collections.Generic;
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

        // constructor
        public Rabbit()
        {
            moveSpeed = _moveSpeed;
            senseRadius = _senseRadius;
            diet = _diet;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, _senseRadius);
        }
    }
}

