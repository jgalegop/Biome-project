using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InputFieldTabInteraction : MonoBehaviour
{
    [SerializeField]
    private List<TMP_InputField> _inputFieldsList = new List<TMP_InputField>();

    private TMP_InputField _selectedInput = null;
    private void Update()
    {
        if (_selectedInput != null)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                int selectedIndex = _inputFieldsList.IndexOf(_selectedInput);
                int nextSelectedIndex = (selectedIndex == _inputFieldsList.Count - 1) ? 0 : selectedIndex + 1;
                _inputFieldsList[nextSelectedIndex].Select();
            }
        }
    }

    public void OnSelectInput(TMP_InputField input)
    {
        _selectedInput = input;
    }

    public void OnDeselectInput(TMP_InputField input)
    {
        _selectedInput = null;
    }
}
