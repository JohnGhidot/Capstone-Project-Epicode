using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairUI : MonoBehaviour
{
    [Header ("References")]
    [SerializeField] private GameObject _crosshair;

    [Header("Behaviour")]
    [SerializeField] private bool _autoHideOnPause = true;

    private void Awake()
    {
        if (_crosshair != null)
        {
            _crosshair.SetActive(true);
        }
    }

    private void Update()
    {
        if (_autoHideOnPause == true)
        {
            bool paused = (Time.timeScale == 0f);
            if (_crosshair != null)
            {
                _crosshair.SetActive(paused == false);
            }
        }        
    }

    public void Show()
    {
        if (_crosshair != null)
        {
             _crosshair.SetActive(true);
        }
    }  
    
    public void Hide()
    {
        if (_crosshair != null)
        {
            _crosshair.SetActive(false);
        }
    }

}
