using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Entities;
using DG.Tweening;

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

    [SerializeField]
    private GameObject _panelImage = null;

    private Animal _boundAnimal;
    private Vector3 _animalGraphicScale;

    private void Awake()
    {
        _animalGraphicScale = _animalGraphic.localScale;
        _panelImage.transform.localScale = Vector3.zero;

        _animalPanel.SetActive(false);
    }

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

            if (DOTween.IsTweening(_panelImage.transform))
                DOTween.Kill(_panelImage.transform);

            _panelImage.transform.DOScale(Vector3.one, 0.4f)
                                 .SetEase(Ease.OutBack);
        }
        else
        {
            _panelImage.transform.DOScale(Vector3.zero, 0.4f)
                                 .SetEase(Ease.InBack)
                                 .OnComplete(DisableAfterTween);
        }
    }

    private void Update()
    {
        if (_animalPanel.activeSelf)
            UpdatePanel();
    }

    private void DisplayStats(Animal animal)
    {
        _moveSpeedNumber.SetText(animal.GetMoveSpeed().ToString("F1") + " m/s");
        _senseRadiusNumber.SetText(animal.GetSenseRadius().ToString("F1") + " m");
        _dietText.SetText(animal.GetDietText());
    }

    private void UpdatePanel()
    {
        if (_boundAnimal == null)
            return;
        _energyBar.fillAmount = _boundAnimal.GetEnergy() / _boundAnimal.MaxEnergy;
        _energyText.SetText(((int)_boundAnimal.GetEnergy()).ToString());
        _animalGraphic.rotation = _boundAnimal.transform.rotation;
        Vector3 scaledLocalScale = Vector3.Scale(_boundAnimal.transform.localScale, _animalGraphicScale);
        _animalGraphic.localScale = scaledLocalScale;
    }

    private void HandleAnimalDeath()
    {
        Bind(null);
    }

    private void DisableAfterTween()
    {
        _animalPanel.SetActive(false);
    }
}
