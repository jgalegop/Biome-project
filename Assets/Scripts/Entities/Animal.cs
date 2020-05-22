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
        private float _adultMoveSpeed;
        protected float senseRadius;
        protected Type diet;

        private float _energy;
        private float _hungerThreshold = 50;

        private bool? _reproductiveUrge = false;
        private float _repUrgeTime = 30f;
        private float _repUrgeTimeVar = 15f;

        private float _forgetFoodTime = 5f;

        public Vector3 AdultScale { get; private set; }
        public Vector3 YoungScale { get; private set; }

        public float MaxEnergy { get; protected set; }

        public LivingBeing TargetFood { get; private set; }
        public LivingBeing FoodIgnored { get; private set; }
        public Animal TargetMate { get; private set; }
        public Animal TargetPredator { get; private set; }

        public event Action OnAnimalDeath = delegate { };
        public event Action OnAnimalWithReproductiveUrge = delegate { };
        public event Action OnAnimalGrowToAdult = delegate { };

        public AnimalFactory AnimalSpawner { get; private set; }

        public bool DebugModeOn { get; private set; }

        public FiniteStateMachine FSM { get; private set; }

        private bool _foodCoroutineRunning = false;
        private Coroutine _foodIgnoreCoroutine = null;

        public int UnsucsesfulPathfinds = 0;


        public override void Spawn()
        {
            base.Spawn();
            FSM = GetComponent<FiniteStateMachine>();
            InitializeFSM();

            _energy = MaxEnergy;

            _adultMoveSpeed = moveSpeed;

            AdultScale = Vector3.one;
            YoungScale = 0.5f * AdultScale;

            GroundYPos = 0.5f; // HARDCODED

            AnimalSpawner = GetComponent<AnimalFactory>();
            AnimalSpawner.SetInstance(gameObject);

            FoodIgnored = null;
        }


        private void Start()
        {
            StatisticsManager.AnimalIsBorn(this);
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
                {typeof(Mating), new Mating(this) },
                {typeof(Fleeing), new Fleeing(this) }
            };

            FSM.SetStates(states);
        }

        public override void Die()
        {
            OnAnimalDeath?.Invoke();
            StatisticsManager.AnimalHasDied(this);
            StopAllCoroutines();
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

        public bool IsHungry()
        {
            return _energy < _hungerThreshold;
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

        public void SetTargetPredator(Animal predator)
        {
            TargetPredator = predator;
        }

        public bool IsAdult()
        {
            return _reproductiveUrge != null;
        }




        //  -----  GET TRAITS  -----  
        public float GetMoveSpeed() { return moveSpeed; }

        public float GetAdultMoveSpeed() { return _adultMoveSpeed; }

        public float GetSenseRadius() { return senseRadius; }

        public Type GetDiet() { return diet; }

        public float GetEnergyLostPerTick() { return 1 + 0.5f * moveSpeed * moveSpeed + 0.2f * senseRadius; }


        public Type GetState()
        {
            if (FSM.CurrentState == null)
                return typeof(Exploring);
            else
                return FSM.CurrentState.GetType();
        }

        public string GetStateName() 
        { 

            if (FSM.CurrentState == FSM.PreviousState)
            {
                if (FSM.CurrentState.StateName != null)
                    return FSM.CurrentState.StateName;
                else
                    return FSM.CurrentState.ToString();
            }
            else
            {
                if (FSM.PreviousState.StateName != null)
                    return FSM.PreviousState.StateName;
                else
                    return FSM.PreviousState.ToString();
            }
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
            _reproductiveUrge = null;

            transform.localScale = YoungScale;
            transform.position += Vector3.up * (GroundYPos + 0.5f * transform.localScale.y - transform.position.y);

            moveSpeed = 0.5f * _adultMoveSpeed;
            StartCoroutine(GrowIntoAdult());
        }

        private IEnumerator GrowIntoAdult()
        {
            float adultTime = UnityEngine.Random.Range(35, 45);
            _reproductiveUrge = null;
            yield return new WaitForSeconds(adultTime);
            AnimalIsAdult();
        }

        public virtual void AnimalIsAdult()
        {
            if (!IsAdult())
            {
                transform.localScale = AdultScale;
                transform.position += Vector3.up * (GroundYPos + 0.5f * transform.localScale.y - transform.position.y);
                moveSpeed = _adultMoveSpeed;
            }
            OnAnimalGrowToAdult?.Invoke();
            StartReproductiveUrge();
        }

        public void SpawnOffspring()
        {
            for (int i = 0; i < UnityEngine.Random.Range(1,4); i++)
            {
                Animal newAnimal = AnimalSpawner.GetNewInstance().GetComponent<Animal>();

                newAnimal.SetTraits(moveSpeed, senseRadius); //overwrite awake SetTraits
                _adultMoveSpeed = moveSpeed;

                newAnimal.AnimalIsYoung();
                newAnimal.transform.position += Vector3.forward * UnityEngine.Random.Range(-1f, 1f) +
                                                Vector3.right * UnityEngine.Random.Range(-1f, 1f);
                newAnimal.transform.SetParent(transform.parent);
                newAnimal.gameObject.name = "Rabbit (born)";
            }
        }

        public virtual void SetTraits(float ms, float sr)
        {
            float _moveSpeedVar = 1f;
            float _senseRadiusVar = 3f;

            moveSpeed = ms + UnityEngine.Random.Range(-_moveSpeedVar, _moveSpeedVar);
            if (moveSpeed < 0)
                moveSpeed = 0;
            _adultMoveSpeed = moveSpeed;

            senseRadius = sr + UnityEngine.Random.Range(-_senseRadiusVar, _senseRadiusVar);
            if (senseRadius < 0)
                senseRadius = 0;
        }


        public void SetFoodIgnored(LivingBeing lb)
        {
            FoodIgnored = lb;
            if (lb != null && this != null && StopIgnoringFood() != null)
            {
                if (_foodIgnoreCoroutine != null && _foodCoroutineRunning)
                    StopCoroutine(_foodIgnoreCoroutine);
                _foodIgnoreCoroutine = StartCoroutine(StopIgnoringFood());
            }
        }

        private IEnumerator StopIgnoringFood()
        {
            _foodCoroutineRunning = true;
            yield return new WaitForSeconds(_forgetFoodTime);
            FoodIgnored = null;
            _foodCoroutineRunning = false;
            _foodIgnoreCoroutine = null;
        }



        public void AnimalDebugToggle()
        {
            DebugModeOn = !DebugModeOn;
        }

        // debug
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, senseRadius);
        }
    }
}