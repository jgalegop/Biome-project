using System;
using System.Collections;
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

        public LivingBeing TargetFood { get; private set; } 

        // IMPROVABLE
        public FiniteStateMachine fsm => GetComponent<FiniteStateMachine>();

        public override void Awake()
        {
            base.Awake();
            InitializeFSM();
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

