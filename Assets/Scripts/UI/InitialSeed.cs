using UnityEngine;
using TMPro;

public class InitialSeed : MonoBehaviour
{
    [SerializeField]
    private MapGenerator _mapGen = null;

    private void Awake()
    {
        TMP_InputField inputField = GetComponent<TMP_InputField>();
        if (_mapGen != null && inputField != null)
            inputField.text = _mapGen.GetSeed().ToString();
    }
}