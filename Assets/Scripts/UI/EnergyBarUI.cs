using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Entities;

public class EnergyBarUI : MonoBehaviour
{
    [SerializeField]
    private GameObject energyBar = null;

    [SerializeField]
    private Image foregroundImage = null;

    [SerializeField]
    private float energyBarOffset = 1f;

    private Animal _boundAnimal = null;

    private Camera _mainCamera = null;
    private RectTransform _canvasRect = null;

    private void Awake()
    {
        _mainCamera = FindObjectOfType<Camera>();
        _canvasRect = transform.parent.GetComponent<RectTransform>();
    }

    public void Bind(Animal animal)
    {
        _boundAnimal = animal;

        if (_boundAnimal != null)
        {
            energyBar.SetActive(true);
        }
        else
        {
            energyBar.SetActive(false);
        }
    }

    private void Update()
    {
        if (energyBar.activeSelf)
        {
            foregroundImage.fillAmount = _boundAnimal.GetEnergy() / _boundAnimal.maxEnergy;

            // convert screen coords
            Vector2 screenPos = _mainCamera.WorldToScreenPoint(_boundAnimal.transform.position + Vector3.up * energyBarOffset);

            screenPos.x *= _canvasRect.rect.width / (float)_mainCamera.pixelWidth;
            screenPos.y *= _canvasRect.rect.height / (float)_mainCamera.pixelHeight;

            // set it
            energyBar.GetComponent<RectTransform>().anchoredPosition = screenPos - _canvasRect.sizeDelta / 2f;
        }
    }  
}
