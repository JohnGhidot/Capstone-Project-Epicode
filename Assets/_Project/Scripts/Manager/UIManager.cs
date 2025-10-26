using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class UIManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _resultPanel;
    [SerializeField] private GameObject _continueButton;
    [SerializeField] private Image _titleImage;
    [SerializeField] private Sprite _youWinSprite;
    [SerializeField] private Sprite _youLoseSprite;

    [Header("Flow")]
    [SerializeField] private int _mainMenuBuildIndex = 0;

    private bool _winShown = false;

    private void Awake()
    {
        if (_resultPanel != null)
        {
            _resultPanel.SetActive(false);
        }

        if (_continueButton != null)
        {
            _continueButton.SetActive(false);
        }

        if ( Time.timeScale != 1f)
        {
            Time.timeScale = 1f;
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OnEnemyDefeated()
    {
        ShowResult(true);
    }

    public void OnPlayerDefeated()
    {
        ShowResult(false);
    }

    private void ShowResult(bool win)
    {
        _winShown = win;

        if (_resultPanel != null)
        {
            _resultPanel.SetActive(true);
        }

        if (_titleImage != null)
        {
            _titleImage.sprite = (win == true) ? _youWinSprite : _youLoseSprite;
            _titleImage.enabled = (_titleImage.sprite != null);
        }


        bool hasNext = HasNextScene();
        if (_continueButton != null)
        {
            if (win == true && hasNext == true)
            {
                _continueButton.SetActive(true);
            }
            else
            {
                _continueButton.SetActive(false);
            }
        }

        Time.timeScale = 0f;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void OnContinueClicked()
    {
        if (_winShown == false)
        {
            return;
        }

        Time.timeScale = 1f;

        int current = SceneManager.GetActiveScene().buildIndex;
        int next = current + 1;
        int total = SceneManager.sceneCountInBuildSettings;

        if (next < total)
        {
            SceneManager.LoadScene(next);
        }
        else
        {
            SceneManager.LoadScene(_mainMenuBuildIndex);
        }
    }

    public void OnRetryClicked()
    {
        Time.timeScale = 1f;
        int current = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(current);
    }

    public void OnMainMenuClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(_mainMenuBuildIndex);
    }

    private bool HasNextScene()
    {
        int current = SceneManager.GetActiveScene().buildIndex;
        int next = current + 1;
        int total = SceneManager.sceneCountInBuildSettings;

        if (next < total)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
