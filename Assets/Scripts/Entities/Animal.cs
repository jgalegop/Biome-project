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
    [RequireComponent(typeof(FiniteStateMachine), typeof(AnimalFactory))]
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

        public AnimalFactory AnimalSpawner { get; private set; }

        public bool DebugModeOn { get; private set; }

        public FiniteStateMachine FSM { get; private set; }


        public override void Awake()
        {
            Spawn();
        }

        public override void Spawn()
        {
            base.Spawn();
            FSM = GetComponent<FiniteStateMachine>();
            InitializeFSM();

            _energy = MaxEnergy;

            Debug.Log("Spawn");

            DefaultScale = transform.localScale;
            Debug.Log(DefaultScale);

            if (IsAdult())
                StartReproductiveUrge();

            AnimalSpawner = GetComponent<AnimalFactory>();
            AnimalSpawner.SetInstance(gameObject);
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

        public virtual AnimalFactory GetFactory()
        {
            return FindObjectOfType<AnimalFactory>();
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

        public virtual void AnimalIsYoung()
        {
            Debug.Log("AnimalIsYoung");
            Debug.Log(DefaultScale);
            transform.localScale = DefaultScale * 0.5f;
            DefaultScale = transform.localScale;
            Debug.Log(DefaultScale);

            transform.position += Vector3.up * (0.5f * DefaultScale.y - transform.position.y);
            moveSpeed *= 0.5f;
            _reproductiveUrge = null;
            StartCoroutine(GrowIntoAdult());
        }

        private IEnumerator GrowIntoAdult()
        {
            float adultTime = UnityEngine.Random.Range(35, 45);
            yield return new WaitForSeconds(adultTime);
            AnimalIsAdult();
        }

        public virtual void AnimalIsAdult()
        {
            if (!IsAdult())
            {
                transform.localScale = DefaultScale * 2f;
                DefaultScale = transform.localScale;
                transform.position += Vector3.up * (0.5f * DefaultScale.y - transform.position.y);
                moveSpeed *= 2f;
            }
            StartReproductiveUrge();
        }

        public void SpawnOffspring()
        {
            for (int i = 0; i < UnityEngine.Random.Range(1,4); i++)
            {
                Animal newAnimal = AnimalSpawner.GetNewInstance().GetComponent<Animal>();
                newAnimal.AnimalIsYoung();
            }
        }


        public void AnimalDebugToggle()
        {
            DebugModeOn = !DebugModeOn;
        }
    }
}

