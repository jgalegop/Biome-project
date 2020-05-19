using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Globalization;

[RequireComponent(typeof(TMP_InputField))]
public class InputFieldController : MonoBehaviour
{
    [SerializeField]
    private Slider _slider = null;
    private TMP_InputField _inputField = null;

    public bool InputFieldUpdated { get; private set; }

    private void Awake()
    {
        _inputField = GetComponent<TMP_InputField>();
        InputFieldUpdated = false;
    }

    public void UpdateSliderFromInputField()
    {
        InputFieldUpdated = true;
        if (_slider.wholeNumbers)
        {
            if (_inputField.text == string.Empty)
                _slider.value = 0;
            else
                _slider.value = int.Parse(_inputField.text);
        }
        else
        {
            if (_inputField.text == string.Empty)
            {
                _slider.value = 0;
            }
            else
            {
                if (_inputField.text != "-")
                {
                    _slider.value = float.Parse(_inputField.text);
                }
            }     
        }
    }

    private void LateUpdate()
    {
        if (InputFieldUpdated)
            InputFieldUpdated = false;
    }
}