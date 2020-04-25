using UnityEngine;

public class PauseButton : ReproductionButton
{
    [SerializeField]
    private PlayButton _playButton = null;
    [SerializeField]
    private ForwardButton _forwardButton = null;

    public void PressButton()
    {
        ReproSettings.SetTimeScale(0f);
        SetIsPressed(true);
        _playButton.SetIsPressed(false);
        _forwardButton.SetIsPressed(false);
    }
}
