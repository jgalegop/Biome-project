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
    private TMP_Text _animalText = null;

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

    [SerializeField]
    private TMP_Text _stateText = null;

    [SerializeField]
    private Image _reproductiveUrgeImage = null;

    [SerializeField]
    private Sprite _repUrgeOn = null;

    [SerializeField]
    private Sprite _repUrgeOff = null;

    private Animal _boundAnimal;
    private Vector3 _animalGraphicScale;
    private Color _reproductiveUrgeImageColor;

    private void Awake()
    {
        _animalGraphicScale = _animalGraphic.localScale;
        _panelImage.transform.localScale = Vector3.zero;

        _animalPanel.SetActive(false);

        _reproductiveUrgeImageColor = _reproductiveUrgeImage.color;
    }

    public void Bind(Animal animal)
    {
        if (_boundAnimal != null)
        {
            _boundAnimal.OnAnimalDeath -= HandleAnimalDeath;
            _boundAnimal.OnAnimalWithReproductiveUrge -= UpdateReproductiveUrgeIcon;
            _boundAnimal.OnAnimalGrowToAdult -= DisplayStats;
        }

        _boundAnimal = animal;

        if (_boundAnimal != null)
        {
            _boundAnimal.OnAnimalDeath += HandleAnimalDeath;
            _boundAnimal.OnAnimalWithReproductiveUrge += UpdateReproductiveUrgeIcon;
            _boundAnimal.OnAnimalGrowToAdult += DisplayStats;

            DisplayStats();
            _animalPanel.SetActive(true);

            if (DOTween.IsTweening(_panelImage.transform))
                DOTween.Kill(_panelImage.transform);

            _panelImage.transform.DOScale(Vector3.one, 0.4f)
                                 .SetEase(Ease.OutBack)
                                 .SetUpdate(true);
            _animalGraphic.GetChild(0).transform.localRotation= _boundAnimal.transform.GetChild(0).transform.localRotation;
            _animalGraphic.GetChild(0).transform.localScale = 0.75f * _boundAnimal.transform.GetChild(0).transform.localScale;
            _animalGraphic.GetComponentInChildren<MeshFilter>().mesh = _boundAnimal.GetComponentInChildren<MeshFilter>().mesh;
            _animalGraphic.GetComponentInChildren<MeshRenderer>().material = _boundAnimal.GetComponentInChildren<MeshRenderer>().material;
            _animalGraphic.GetComponentInChildren<MeshRenderer>().material.color = _boundAnimal.GetComponentInChildren<MeshRenderer>().material.color;
        }
        else
        {
            _panelImage.transform.DOScale(Vector3.zero, 0.4f)
                                 .SetEase(Ease.InBack)
                                 .OnComplete(DisableAfterTween)
                                 .SetUpdate(true);
        }
    }

    private void Update()
    {
        if (_animalPanel.activeSelf)
            UpdatePanel();
    }

    private void DisplayStats()
    {
        _animalText.SetText(_boundAnimal.GetSpecieText().ToString());
        _moveSpeedNumber.SetText(_boundAnimal.GetMoveSpeed().ToString("F1") + " m/s");
        _senseRadiusNumber.SetText(_boundAnimal.GetSenseRadius().ToString("F1") + " m");
        _dietText.SetText(_boundAnimal.GetDietText());
        UpdateReproductiveUrgeIcon();
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
        _stateText.SetText("(" + _boundAnimal.GetStateName() + ")");
    }

    private void UpdateReproductiveUrgeIcon()
    {
        if (_boundAnimal.IsAdult())
        {
            _reproductiveUrgeImage.color = _reproductiveUrgeImageColor;
            if (_boundAnimal.GetReproductiveUrge())
                _reproductiveUrgeImage.sprite = _repUrgeOn;
            else
                _reproductiveUrgeImage.sprite = _repUrgeOff;
        }
        else
        {
            _reproductiveUrgeImage.sprite = null;
            _reproductiveUrgeImage.color = Color.clear;
        }
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
