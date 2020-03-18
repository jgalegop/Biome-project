﻿using System;
using System.Collections.Generic;
using UnityEngine;


namespace Entities
{
    /// <summary>
    /// Abstract class with the AI that dominates all animals. 
    /// Inherits from livingbeing in order to spawn and kill an animal entity.
    /// Controls energy and defines different animal traits (move speed, sense radius, diet...)
    /// </summary>
    [RequireComponent(typeof(FiniteStateMachine))]
    public abstract class Animal : LivingBeing
    {
        protected float moveSpeed;
        protected float senseRadius;
        protected Type diet;

        private float _energy;

        public float MaxEnergy { get; protected set; }

        public LivingBeing TargetFood { get; private set; }

        public event Action OnAnimalDeath = delegate { };

        // IMPROVABLE
        public FiniteStateMachine FSM => GetComponent<FiniteStateMachine>();

        public override void Awake()
        {
            base.Awake();
            InitializeFSM();

            _energy = MaxEnergy;
        }

        private void InitializeFSM()
        {
            var states = new Dictionary<Type, State>()
            {
                {typeof(Exploring), new Exploring(this) },
                {typeof(ChasingFood), new ChasingFood(this) },
                {typeof(Eating), new Eating(this) }
            };

            FSM.SetStates(states);
        }

        public override void Die()
        {
            OnAnimalDeath?.Invoke();
            base.Die();
        }

        public void SetTargetFood(LivingBeing target)
        {
            TargetFood = target;
        }


        public virtual void MoveTick(Vector3 destination)
        {
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }


        //  -----  ENERGY  -----  
        public void ModifyEnergy(float energyLost)
        {
            _energy += energyLost * Time.deltaTime;
            if (_energy <= 0)
                Die();
        }

        public float GetEnergy()
        {
            return _energy;
        }



        //  -----  GET TRAITS  -----  
        public float GetMoveSpeed() { return moveSpeed; }

        public float GetSenseRadius() { return senseRadius; }

        public Type GetDiet() { return diet; }

        
        public string GetDietText()
        {
            String[] subTypes = GetDiet().ToString().Split('.');
            return subTypes[subTypes.Length - 1];
        }
    }
}

