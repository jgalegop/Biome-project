using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Slider))]
public class SliderController : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField _inputField = null;
    private InputFieldController _inputFieldController = null;
    private Slider _slider = null;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
        _inputFieldController = _inputField.GetComponent<InputFieldController>();
    }

    public void UpdateTextFromSlider()
    {
        if (_inputFieldController.InputFieldUpdated)
            return;

        if (_slider.wholeNumbers)
        {
            _inputField.text = _slider.value.ToString();
        }
        else
        {
            _inputField.text = _slider.value.ToString("##0.00");
        }
    }
}