using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Entities;

public class EnergyBarUI : MonoBehaviour
{
    [SerializeField]
    private Image foregroundImage = null;

    [SerializeField]
    private Animal animal = null;

    private Camera mainCamera = null;

    private void Awake()
    {
        mainCamera = FindObjectOfType<Camera>();
    }

    private void Update()
    {
        foregroundImage.fillAmount = animal.GetEnergy() / animal.maxEnergy;
    }

    private void LateUpdate()
    {
        transform.LookAt(mainCamera.transform);
        transform.Rotate(0, 180, 0);
    }
}
