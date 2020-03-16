using System;
using System.Collections;
using System.Collections.Generic;
using Entities;
using UnityEngine;

public class AnimalGUIController : MonoBehaviour
{
    [SerializeField]
    private AnimalPanel _animalPanel = null;

    [SerializeField]
    private EnergyBarUI _energyBarUI = null;

    //private EnergyBarUI _energyBarUI;

    private void Awake()
    {
        ClickSelectController.OnSelectedAnimalChanged += HandleSelectedAnimalChanged;
        HandleSelectedAnimalChanged(ClickSelectController.SelectedAnimal);
    }

    private void OnDestroy()
    {
        ClickSelectController.OnSelectedAnimalChanged -= HandleSelectedAnimalChanged;
    }

    private void HandleSelectedAnimalChanged(Animal animal)
    {
        if (_animalPanel != null)
            _animalPanel.Bind(animal);

        if (_energyBarUI != null)
            _energyBarUI.Bind(animal);
    }
}
