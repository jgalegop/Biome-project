using Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatisticsManager : MonoBehaviour
{
    public static event Action<Animal> OnAnimalNumberIncreased = delegate { };
    public static event Action<Animal> OnAnimalNumberDecreased = delegate { };

    private static StatisticsManager instance;

    private int _rabbitNumber = 0;
    private float _initialTime;

    public float MaxSpeed { get; private set; }

    public float DataTimeInterval = 10f;

    public Dictionary<float, DataPoint> PopulationInTime = new Dictionary<float, DataPoint>();

    public List<float> TimeStamps = new List<float>();
    public List<float> MoveSpeeds = new List<float>();
    public List<float> SenseRadii = new List<float>();

    public static event Action<float> OnTimeDataSaved = delegate { };

    private void Awake()
    {
        instance = this;
        _initialTime = Time.time;
        MaxSpeed = 10f;
    }

    private void Start()
    {
        StartCoroutine(GatherData());
    }

    private IEnumerator GatherData()
    {
        while (true)
        {
            yield return new WaitForSeconds(DataTimeInterval);
            float time = Time.time - _initialTime;
            PopulationInTime.Add(time, new DataPoint(time, _rabbitNumber, GetAverageSpeed(), GetAverageSenseRadius()));
            TimeStamps.Add(time);
            OnTimeDataSaved?.Invoke(time);
        }
    }

    public static void AnimalIsBorn(Animal animal)
    {
        OnAnimalNumberIncreased?.Invoke(animal);
        instance._rabbitNumber++;
        instance.MoveSpeeds.Add(animal.GetAdultMoveSpeed());
        instance.SenseRadii.Add(animal.GetSenseRadius());
    }

    public static void AnimalHasDied(Animal animal)
    {
        OnAnimalNumberDecreased?.Invoke(animal);
        instance._rabbitNumber--;
        instance.MoveSpeeds.Remove(animal.GetAdultMoveSpeed());
        instance.SenseRadii.Remove(animal.GetSenseRadius());
    }

    public int GetRabbitNumber()
    {
        return _rabbitNumber;
    }

    public float GetAverageSpeed()
    {
        float moveSpeedSum = 0;
        foreach (float ms in MoveSpeeds)
        {
            moveSpeedSum += ms;
        }
        return moveSpeedSum / (float) _rabbitNumber;
    }

    public float GetAverageSenseRadius()
    {
        float senseRadiusSum = 0;
        foreach (float sr in SenseRadii)
        {
            senseRadiusSum += sr;
        }
        return senseRadiusSum / (float)_rabbitNumber;
    }

    public void SetMaxSpeed(float newSpeed)
    {
        MaxSpeed = newSpeed;
    }
}


public struct DataPoint
{
    public float Time;
    public int Population;
    public float Speed;
    public float SenseRadius;

    public DataPoint(float time, int pop, float speed, float senseRad)
    {
        Time = time;
        Population = pop;
        Speed = speed;
        SenseRadius = senseRad;
    }
}