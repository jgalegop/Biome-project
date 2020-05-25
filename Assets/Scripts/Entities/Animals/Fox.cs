using UnityEngine;
using System;
using Random = UnityEngine.Random;

namespace Entities
{
    public class Fox : Animal
    {
        // traits
        private float _moveSpeed = 3.5f;
        private float _senseRadius = 20f;

        // diet
        private readonly Type _diet = typeof(Rabbit);

        // energy
        private readonly float _maxEnergy = 100;

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

        public override void SpawnOffspring()
        {
            for (int i = 0; i < Random.Range(1, 2); i++)
            {
                Animal newAnimal = AnimalSpawner.GetNewInstance().GetComponent<Animal>();

                newAnimal.SetTraits(moveSpeed, senseRadius); //overwrite awake SetTraits
                adultMoveSpeed = moveSpeed;

                newAnimal.AnimalIsYoung();
                newAnimal.transform.position += Vector3.forward * Random.Range(-1f, 1f) +
                                                Vector3.right * Random.Range(-1f, 1f);
                newAnimal.transform.SetParent(transform.parent);
                newAnimal.gameObject.name = "Fox (born)";
            }
        }

        public override void MoveTick(Vector3 destination)
        {
            base.MoveTick(destination);
        }

        public override void MoveTick(Vector3 destination, float speedModifier)
        {
            base.MoveTick(destination, speedModifier);
        }
    }
}

