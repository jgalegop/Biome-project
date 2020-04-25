using UnityEngine;

public class ForwardButton : ReproductionButton
{
    [SerializeField]
    private PauseButton _pauseButton = null;
    [SerializeField]
    private PlayButton _playButton = null;

    private int _timesPressed = 0;

    public void PressButton()
    {
        _timesPressed++;
        if (_timesPressed < 3)
        {
            ReproSettings.SetTimeScale(_timesPressed * 2f);
            SetIsPressed(true);
            _playButton.SetIsPressed(false);
            _pauseButton.SetIsPressed(false);
        }   
        else if (_timesPressed == 3)
        {
            _timesPressed = 0;
            _playButton.PressButton();
        }
    }
}
