using UnityEngine;
using UnityEngine.UI;

public class ReproductionButton : Button
{
    public ReproductionSettings ReproSettings;

    [SerializeField]
    private bool _isPressed = false;
    [SerializeField]
    private Color _pressedColor = Color.white;

    private Image _image;
    private Color _defaultColor;

    private void Start()
    {
        _image = GetComponent<Image>();
        _defaultColor = _image.color;
        if (_isPressed)
            _image.color = _pressedColor;
    }


    //public override void ChangeSizeOnEnter()
    //{
    //    if (!_isPressed)
    //        base.ChangeSizeOnEnter();
    //}

    //public override void ChangeSizeOnExit()
    //{
    //    if (!_isPressed)
    //        base.ChangeSizeOnExit();
    //}

    public void SetIsPressed(bool isPressed)
    {
        if (_isPressed && _isPressed != isPressed)
        {
            _image.color = _defaultColor;
        }
        if (!_isPressed && _isPressed != isPressed)
        {
            _image.color = _pressedColor;
        }
        _isPressed = isPressed;
    }
}
