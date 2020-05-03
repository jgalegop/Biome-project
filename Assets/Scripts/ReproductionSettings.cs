using UnityEngine;

public class ReproductionSettings : MonoBehaviour
{
    [SerializeField]
    private float _timeScale = 1f; // for debugging

    private int _timesPressedForward = 0;

    private SelectableGroup _selectableGroup = null;

    [SerializeField]
    private SelectableButton _playButton = null;
    [SerializeField]
    private SelectableButton _forwardButton = null;

    private void Start()
    {
        _selectableGroup = GetComponent<SelectableGroup>();
        if (_selectableGroup == null)
            Debug.LogError("Reproduction menu must have a SelectableGroup component");
    }

    public void SetTimeScale(float timeScale)
    {
        _timeScale = timeScale;
        Time.timeScale = _timeScale;
    }

    public void PressForwardButton()
    {
        _timesPressedForward++;
        if (_timesPressedForward < 3)
        {
            SetTimeScale(_timesPressedForward * 2f);
            if (_timesPressedForward < 2)
                _forwardButton.GetComponent<ShowTooltip>().SetCustomText("Increase simulation speed x" + 2 * _timeScale);
            else
                _forwardButton.GetComponent<ShowTooltip>().SetCustomText("Return to normal simulation speed");
        }
        else if (_timesPressedForward == 3)
        {
            _selectableGroup.OnButtonDeselected(_forwardButton);
            _selectableGroup.OnButtonSelected(_playButton);
            PressPlayButton();
            _timesPressedForward = 0;
        }
    }

    public void PressPauseButton()
    {
        _timesPressedForward = 0;
        SetTimeScale(0f);
    }

    public void PressPlayButton()
    {
        _timesPressedForward = 0;
        SetTimeScale(1f);
    }
}