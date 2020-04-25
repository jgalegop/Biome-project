using UnityEngine;

public class ReproductionSettings : MonoBehaviour
{
    [SerializeField]
    private float _timeScale = 1f;
    // for debugging

    public void SetTimeScale(float timeScale)
    {
        _timeScale = Time.timeScale;
        Time.timeScale = timeScale;
    }
}