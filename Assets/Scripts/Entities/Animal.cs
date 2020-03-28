using System;
using System.Collections;
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
        private bool? _reproductiveUrge = false;
        private float _repUrgeTime = 30f;
        private float _repUrgeTimeVar = 15f;

        public Vector3 DefaultScale { get; private set; }
        public float MaxEnergy { get; protected set; }

        public LivingBeing TargetFood { get; private set; }

        public Animal TargetMate { get; private set; }

        public event Action OnAnimalDeath = delegate { };

        public event Action OnAnimalWithReproductiveUrge = delegate { };

        public bool DebugModeOn { get; private set; }

        public FiniteStateMachine FSM { get; private set; }

        public override void Awake()
        {
            base.Awake();
            FSM = GetComponent<FiniteStateMachine>();
            InitializeFSM();

            _energy = MaxEnergy;
            DefaultScale = transform.localScale;

            if (IsAdult())
                StartReproductiveUrge();
            
        }

        private void InitializeFSM()
        {
            var states = new Dictionary<Type, State>()
            {
                {typeof(Exploring), new Exploring(this) },
                {typeof(ChasingFood), new ChasingFood(this) },
                {typeof(Eating), new Eating(this) },
                {typeof(GoingForMate), new GoingForMate(this) },
                {typeof(Mating), new Mating(this) }
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
            else if (_energy > 100)
                _energy = 100;
        }

        public float GetEnergy()
        {
            return _energy;
        }

        //  -----  REPRODUCTIVE URGE  -----
        public bool GetReproductiveUrge()
        {
            return _reproductiveUrge.Value;
        }

        public void StartReproductiveUrge()
        {
            _reproductiveUrge = false;
            StartCoroutine(ReproductiveUrgeRoutine());
        }

        private IEnumerator ReproductiveUrgeRoutine()
        {
            float timeVariation = UnityEngine.Random.Range(-_repUrgeTimeVar, _repUrgeTimeVar);
            yield return new WaitForSeconds(_repUrgeTime + timeVariation);
            _reproductiveUrge = true;
            OnAnimalWithReproductiveUrge?.Invoke();
        }

        public void StopReproductiveUrge()
        {   
            if (_reproductiveUrge.Value)
            {
                _reproductiveUrge = false;
                OnAnimalWithReproductiveUrge?.Invoke();
                StartReproductiveUrge();
            }
        }

        public void SetTargetMate(Animal mate)
        {
            TargetMate = mate;
        }


        public bool IsAdult()
        {
            return _reproductiveUrge != null;
        }


        public void AnimalIsNotAdult()
        {
            _reproductiveUrge = null;
        }




        //  -----  GET TRAITS  -----  
        public float GetMoveSpeed() { return moveSpeed; }

        public float GetSenseRadius() { return senseRadius; }

        public Type GetDiet() { return diet; }

        public float GetEnergyLostPerTick() { return 0.5f * moveSpeed * moveSpeed; }

        
        public Type GetState()
        {
            return FSM.CurrentState.GetType();
        }

        public string GetStateName() 
        { 
            if (FSM.CurrentState.StateName != null)
                return FSM.CurrentState.StateName;
            else
                return FSM.CurrentState.ToString();

        }
        
        public string GetDietText()
        {
            String[] subTypes = GetDiet().ToString().Split('.');
            return subTypes[subTypes.Length - 1];
        }


        private int _tickOfDeath = 0;

        public override void BeingEaten()
        {
            _tickOfDeath++;
            if (_tickOfDeath > 100)
                Die();
        }

        public void AnimalIsYoung()
        {
            DefaultScale *= 0.5f;
            transform.position += Vector3.up * (0.5f * DefaultScale.y - transform.position.y);
            moveSpeed *= 0.5f;
            _reproductiveUrge = null;
        }




        public void AnimalDebugToggle()
        {
            DebugModeOn = !DebugModeOn;
        }
    }
}

