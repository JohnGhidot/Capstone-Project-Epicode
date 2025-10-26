using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class MenuManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _mainMenuRoot;
    [SerializeField] private GameObject _creditsPanel;

    [Header("Flow")]
    [SerializeField] private int _firstFightBuildIndex = 1;


    private void Awake()
    {
        if (Time.timeScale != 1f)
        {
            Time.timeScale = 1f;
        }

        if (_mainMenuRoot != null)
        {
            _mainMenuRoot.SetActive(true);
        }

        if (_creditsPanel != null)
        {
            _creditsPanel.SetActive(false);
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void OnStartGameClicked()
    {
        if (_firstFightBuildIndex < 0)
        {
            _firstFightBuildIndex = 1;
        }

        SceneManager.LoadScene(_firstFightBuildIndex);
    }

    public void OnCreditsClicked()
    {
        if (_mainMenuRoot != null)
        {
            _mainMenuRoot.SetActive(false);
        }

        if (_creditsPanel != null)
        {
            _creditsPanel.SetActive(true);
        }
    }

    public void OnBackFromCreditsClicked()
    {
        if (_creditsPanel != null)
        {
            _creditsPanel.SetActive(false);
        }

        if (_mainMenuRoot != null)
        {
            _mainMenuRoot.SetActive(true);
        }
    }

    public void OnExitGameClicked()
    {
        Application.Quit();
    }

}
