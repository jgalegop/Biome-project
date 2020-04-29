using UnityEngine;

public class HiddenAtStart : MonoBehaviour
{
    [SerializeField]
    private bool _startsHidden = true;
    private void Awake()
    {
        if (_startsHidden)
            transform.localScale = Vector3.zero;
    }
}
