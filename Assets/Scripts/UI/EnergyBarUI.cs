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
    private float energyBarOffset = 1.75f;

    private Animal _boundAnimal = null;

    private Camera _mainCamera = null;
    private RectTransform _canvasRect = null;
    private RectTransform _energyBarRect = null;

    private void Awake()
    {
        _mainCamera = FindObjectOfType<Camera>();
        _canvasRect = transform.parent.GetComponent<RectTransform>();
        _energyBarRect = energyBar.GetComponent<RectTransform>();
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
            energyBar.SetActive(true);
        }
        else
        {
            energyBar.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        if (energyBar.activeSelf)
        {
            foregroundImage.fillAmount = _boundAnimal.GetEnergy() / _boundAnimal.MaxEnergy;

            _energyBarRect.anchoredPosition = WorldToCameraSpace(_boundAnimal.transform.position + Vector3.up * energyBarOffset);
        }
    }  

    private Vector2 WorldToCameraSpace(Vector3 pos)
    {
        Vector2 screenPos = _mainCamera.WorldToScreenPoint(pos);

        screenPos.x *= _canvasRect.rect.width / (float)_mainCamera.pixelWidth;
        screenPos.y *= _canvasRect.rect.height / (float)_mainCamera.pixelHeight;

        return screenPos - _canvasRect.sizeDelta / 2f;
    }

    private void HandleAnimalDeath()
    {
        Bind(null);
    }
}
