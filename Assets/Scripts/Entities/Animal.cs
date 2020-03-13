using System;
using System.Collections.Generic;
using UnityEngine;


namespace Entities
{
    /// <summary>
    /// Abstract class with the AI that dominates all animals. 
    /// Inherits from livingbeing in order to spawn and kill an animal entity.
    /// </summary>
    [RequireComponent(typeof(FiniteStateMachine))]
    public abstract class Animal : LivingBeing
    {
        protected float moveSpeed;
        protected float senseRadius;
        protected Type diet;

        [SerializeField] // for debugging
        private float energy;

        public float maxEnergy { get; protected set; }

        public LivingBeing TargetFood { get; private set; } 

        // IMPROVABLE
        public FiniteStateMachine fsm => GetComponent<FiniteStateMachine>();

        public override void Awake()
        {
            base.Awake();
            InitializeFSM();

            energy = maxEnergy;
        }

        private void InitializeFSM()
        {
            var states = new Dictionary<Type, State>()
            {
                {typeof(Exploring), new Exploring(this) },
                {typeof(ChasingFood), new ChasingFood(this) },
                {typeof(Eating), new Eating(this) }
            };

            GetComponent<FiniteStateMachine>().SetStates(states);

        }

        public void SetTargetFood(LivingBeing target)
        {
            TargetFood = target;
        }

        public void LoseEnergy(float energyLost)
        {
            energy -= energyLost * Time.deltaTime;
            if (energy <= 0)
            {
                Die();
            }
        }

        public void GainEnergy(float energyGained)
        {
            energy += energyGained * Time.deltaTime;
        }

        public float GetEnergy()
        {
            return energy;
        }



        // get animal traits
        public float GetMoveSpeed()
        {
            return moveSpeed;
        }

        public float GetSenseRadius()
        {
            return senseRadius;
        }

        public Type GetDiet()
        {
            return diet;
        }
    }

}

