using Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatisticsManager : MonoBehaviour
{
    private static StatisticsManager instance;

    public static event Action<AnimalData> OnAnimalNumberIncreased = delegate { };
    public static event Action<AnimalData> OnAnimalNumberDecreased = delegate { };

    public static event Action<float> OnTimeDataSaved = delegate { };

    private float _initialTime;
    public float DataTimeInterval = 10f;
    private bool _firstAnimalIsBorn = false;

    public List<float> TimeStamps = new List<float>();
    public Dictionary<float, DataPoint> PopulationInTime = new Dictionary<float, DataPoint>();

    public List<AnimalData> RabbitData = new List<AnimalData>();



    public float MaxSpeed { get; private set; }
    public static event Action<float> OnMaxSpeedChanged = delegate { };
    public static event Action<float> OnMaxSenseRadiusChanged = delegate { };

    

    

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
        
        MaxSpeed = 10f;
    }

    private IEnumerator GatherData()
    {
        _initialTime = Time.time;
        while (true)
        {
            yield return new WaitForSeconds(DataTimeInterval);
            float time = Time.time - _initialTime;

            // FOR NOW ONLY WORKING FOR RABBIT
            AnimalData averageData = GetAverageAnimalData(RabbitData);
            PopulationInTime.Add(time, new DataPoint(time, RabbitData.Count, averageData.speed, averageData.senseRadius));
            TimeStamps.Add(time);
            OnTimeDataSaved?.Invoke(time);
        }
    }

    public static void AnimalIsBorn(Animal animal)
    {
        AnimalData aData = new AnimalData(animal.GetAdultMoveSpeed(), animal.GetSenseRadius());
        instance.RabbitData.Add(aData);
        OnAnimalNumberIncreased?.Invoke(aData);
        

        if (!instance._firstAnimalIsBorn)
        {
            instance.StartCoroutine(instance.GatherData());
            instance._firstAnimalIsBorn = true;
        }
    }

    public static void AnimalHasDied(Animal animal)
    {
        AnimalData aData = new AnimalData(animal.GetAdultMoveSpeed(), animal.GetSenseRadius());
        instance.RabbitData.Remove(aData);
        OnAnimalNumberDecreased?.Invoke(aData);
    }

    private AnimalData GetAverageAnimalData(List<AnimalData> animalData)
    {
        float moveSpeedSum = 0;
        float senseRadiusSum = 0;
        foreach (AnimalData ad in animalData)
        {
            moveSpeedSum += ad.speed;
            senseRadiusSum += ad.senseRadius;
        }
        float avgSpeed = moveSpeedSum / (float)animalData.Count;
        float avgSenseRadius = senseRadiusSum / (float)animalData.Count;
        return new AnimalData(avgSpeed, avgSenseRadius);
    }

    public void SetMaxSpeed(float newSpeed)
    {
        Debug.Log("new max speed is set");
        MaxSpeed = newSpeed;
        OnMaxSpeedChanged?.Invoke(newSpeed);
    }
}


public struct DataPoint
{
    public float time;
    public int population;
    public float averageSpeed;
    public float averageSenseRadius;

    public DataPoint(float time, int population, float averageSpeed, float averageSenseRadius)
    {
        this.time = time;
        this.population = population;
        this.averageSpeed = averageSpeed;
        this.averageSenseRadius = averageSenseRadius;
    }
}

public struct AnimalData
{
    public float speed;
    public float senseRadius;

    public AnimalData(float speed, float senseRadius)
    {
        this.speed = speed;
        this.senseRadius = senseRadius;
    }
}