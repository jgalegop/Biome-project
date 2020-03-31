using UnityEngine;
using System;
using Random = UnityEngine.Random;

namespace Entities
{
    public class Rabbit : Animal
    {
        // traits
        private float _moveSpeed = 2.5f;
        private float _senseRadius = 10f;

        // diet
        private readonly Type _diet = typeof(Plant);

        // energy
        private readonly float _maxEnergy = 100;

        // movement
        private RabbitMovement _rabbitMovement;


        public override void Spawn()
        {
            diet = _diet;
            MaxEnergy = _maxEnergy;

            SetTraits(_moveSpeed, _senseRadius);

            base.Spawn();

            _rabbitMovement = GetComponent<RabbitMovement>();
        }



        public override void MoveTick(Vector3 destination)
        {
            if (_rabbitMovement != null)
                _rabbitMovement.Tick(destination); 
            else
                base.MoveTick(destination);
        }

        public override void AnimalIsYoung()
        {
            base.AnimalIsYoung();
            _rabbitMovement.SetDefaultScale(YoungScale);
            _rabbitMovement.SetMoveParams();
        }

        public override void AnimalIsAdult()
        {
            base.AnimalIsAdult();
            _rabbitMovement.SetDefaultScale(AdultScale);
            _rabbitMovement.SetMoveParams();
        }


        // debug
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, _senseRadius);
        }
    }
}

