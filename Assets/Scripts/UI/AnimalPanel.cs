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
    private TMP_Text _moveSpeedNumber = null;

    [SerializeField]
    private TMP_Text _senseRadiusNumber = null;

    [SerializeField]
    private TMP_Text _dietText = null;

    [SerializeField]
    private RectTransform _animalGraphic = null;

    [SerializeField]
    private GameObject _animalPanel = null;

    private Animal _boundAnimal;

    public void Bind(Animal animal)
    {
        if (_boundAnimal != null)
        {
            _boundAnimal.OnAnimalDeath -= HandleAnimalDeath;
        }

        _boundAnimal = animal;

        if (_boundAnimal != null)
        {
            _boundAnimal.OnAnimalDeath += HandleAnimalDeath;
            DisplayStats(_boundAnimal);
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

    private void DisplayStats(Animal animal)
    {
        _moveSpeedNumber.SetText(animal.GetMoveSpeed().ToString("F1"));
        _senseRadiusNumber.SetText(animal.GetSenseRadius().ToString("F1"));
        _dietText.SetText(animal.GetDietText());
    }

    private void UpdateHealth()
    {
        _energyBar.fillAmount = _boundAnimal.GetEnergy() / _boundAnimal.maxEnergy;
        _energyText.SetText(((int)_boundAnimal.GetEnergy()).ToString());
        _animalGraphic.rotation = _boundAnimal.transform.rotation;
    }

    private void HandleAnimalDeath()
    {
        Bind(null);
    }
}
