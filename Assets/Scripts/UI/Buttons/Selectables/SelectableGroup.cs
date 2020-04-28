using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableGroup : MonoBehaviour
{
    private List<SelectableButton> _selectableButtons = null;

    [SerializeField]
    private Color _idleColor = Color.white;
    [SerializeField]
    private Color _hoverColor = Color.white;
    [SerializeField]
    private Color _selectedColor = Color.white;

    [SerializeField]
    private Color _spriteDefaultColor = Color.white;
    [SerializeField]
    private Color _spriteHoverColor = Color.white;
    [SerializeField]
    private Color _spriteSelectedColor = Color.white;

    private SelectableButton _selectedButton;

    public void BindSelectable(SelectableButton selectable)
    {
        if (_selectableButtons == null)
            _selectableButtons = new List<SelectableButton>();
        _selectableButtons.Add(selectable);
    }

    public void OnButtonEnter(SelectableButton selectable)
    {
        ResetButtons();
        if (_selectedButton != null && _selectedButton == selectable) { return; }
        selectable.ButtonImage.color = _hoverColor;
        selectable.ChangeInImageColor(_spriteHoverColor);
    }

    public void OnButtonExit(SelectableButton selectable)
    {
        ResetButtons();
    }

    public void OnButtonSelected(SelectableButton selectable)
    {
        if (_selectedButton != null)
        {
            _selectedButton.Deselect();
            DeselectFunction(_selectedButton);
            _selectedButton = null;
        }

        ResetButtons();

        _selectedButton = selectable;
        selectable.ButtonImage.color = _selectedColor;
        selectable.ChangeInImageColor(_spriteSelectedColor);
        selectable.Select();
        SelectFunction(selectable);
    }

    public void OnButtonDeselected(SelectableButton selectable)
    {
        selectable.Deselect();
        DeselectFunction(selectable);
        _selectedButton = null;
        ResetButtons();
    }

    private void SelectFunction(SelectableButton selectable)
    {

    }

    private void DeselectFunction(SelectableButton selectable)
    {

    }

    public void ResetButtons()
    {
        foreach (SelectableButton selectable in _selectableButtons)
        {
            if (_selectedButton != null && _selectedButton == selectable) { continue; }
            selectable.ButtonImage.color = _idleColor;
            selectable.ChangeInImageColor(_spriteDefaultColor);
        }
    }

    private IEnumerator DisableMenu(GameObject menu)
    {
        yield return new WaitForSeconds(0.4f);
        menu.SetActive(false);
    }
}
