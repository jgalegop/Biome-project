using UnityEngine;

public class PlayButton : ReproductionButton
{
    [SerializeField]
    private PauseButton _pauseButton = null;
    [SerializeField]
    private ForwardButton _forwardButton = null;

    public void PressButton()
    {
        ReproSettings.SetTimeScale(1f);
        SetIsPressed(true);
        _pauseButton.SetIsPressed(false);
        _forwardButton.SetIsPressed(false);
    }
}
