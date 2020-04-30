using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class DisplayText : MonoBehaviour
{
    private TextMeshProUGUI _tmpro;

    [SerializeField]
    private GameObject _iDisplayableText = null;

    public enum TypeToDisplay { Float, Int, String }

    [SerializeField]
    private TypeToDisplay _typeToDisplay = TypeToDisplay.Float;

    private IDisplayableText _displayable;

    private void Start()
    {
        _tmpro = GetComponent<TextMeshProUGUI>();
        if (_iDisplayableText == null || _iDisplayableText.GetComponent<IDisplayableText>() == null)
        {
            Debug.LogError("GameObject must inherit from IDisplayableText");
        }
        else
        {
            _displayable = _iDisplayableText.GetComponent<IDisplayableText>();
            Display();
        }
    }

    private void Update()
    {
        if (TextChanges())
            Display();
    }

    public void Display()
    {
        if (_typeToDisplay == TypeToDisplay.Float)
        {
            //Debug.Log(_displayable.SetText<float>().ToString("#.#"));
            _tmpro.text = _displayable.SetText<float>().ToString("0.0");
            //Debug.Log(_tmpro.text);
        }   
        else if (_typeToDisplay == TypeToDisplay.Int)
        {
            _tmpro.text = _displayable.SetText<int>().ToString();
        }
        else if (_typeToDisplay == TypeToDisplay.String)
        {
            _tmpro.text = _displayable.SetText<string>();
        }
        else
        {
            Debug.LogError("Invalid type to display. Check return type of function SetText of IDisplayable");
        }
    }

    private bool TextChanges()
    {
        if (_tmpro.text != _displayable.SetText<float>().ToString("#.#"))
            return true;
        else
            return false;
    }
}
