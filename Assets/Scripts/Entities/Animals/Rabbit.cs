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

        // material
        private Material _mat;

        [SerializeField]
        private Color[] _possibleColors = null;


        public override void Spawn()
        {
            diet = _diet;
            MaxEnergy = _maxEnergy;

            SetTraits(_moveSpeed, _senseRadius);

            base.Spawn();

            _rabbitMovement = GetComponent<RabbitMovement>();

            SetColor();
        }

        private void SetColor()
        {
            _mat = GetComponentInChildren<MeshRenderer>().material;
            int i = Random.Range(0, _possibleColors.Length);

            // special color
            int j = Random.Range(0, 255);

            if (j == 0)
                _mat.color = new Color(227f / 255f, 138f / 255f, 167f / 255f); // new Color(222, 150, 171);
            else
                _mat.color = _possibleColors[i];
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
    }
}

