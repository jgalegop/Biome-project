using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    [SerializeField]
    private float timeScale = 1f;

    public enum EnergyDisplayMode {All, OnSelected};

    private void Update()
    {
        Time.timeScale = timeScale;
    }
}