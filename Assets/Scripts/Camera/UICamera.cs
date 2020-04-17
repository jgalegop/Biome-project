using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICamera : MonoBehaviour
{
    [SerializeField]
    private Transform _mainCam = null;

    private void Awake()
    {
        if (_mainCam == null)
            Debug.LogWarning("UI camera does not have a main camera to follow");
    }

    private void Update()
    {
        transform.position = _mainCam.position;
        transform.rotation = _mainCam.rotation;
    }
}
