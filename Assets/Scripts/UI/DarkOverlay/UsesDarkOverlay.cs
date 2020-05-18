using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsesDarkOverlay : MonoBehaviour
{
    [SerializeField]
    private DarkOverlay _darkOverlay = null;

    private void OnEnable()
    {
        if (_darkOverlay != null)
        {
            _darkOverlay.gameObject.SetActive(true);
        }
    }

    private void OnDisable()
    {
        if (_darkOverlay != null)
        {
            _darkOverlay.gameObject.SetActive(false);
        }
    }
}
