using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Entities;

public class AnimalPanel : MonoBehaviour
{
    [SerializeField]
    private Image _energyBar = null;

    [SerializeField]
    private TMP_Text _energyText = null;

    [SerializeField]
    private RectTransform _animalGraphic = null;

    [SerializeField]
    private GameObject _animalPanel = null;

    private Animal _boundAnimal;

    public void Bind(Animal animal)
    {
        _boundAnimal = animal;

        if (_boundAnimal != null)
        {
            _animalPanel.SetActive(true);
        }
        else
        {
            _animalPanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (_animalPanel.activeSelf)
        {
            UpdateHealth();
        }
    }

    private void UpdateHealth()
    {
        _energyBar.fillAmount = _boundAnimal.GetEnergy() / _boundAnimal.maxEnergy;
        _energyText.SetText(((int)_boundAnimal.GetEnergy()).ToString());
        _animalGraphic.rotation = _boundAnimal.transform.rotation;
    }
}
