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
    [SerializeField] private GameObject _pausePanel;

    [Header("Crosshair")]
    [SerializeField] private CrosshairUI _crosshair;

    [Header("Flow")]
    [SerializeField] private int _mainMenuBuildIndex = 0;

    private bool _winShown = false;
    private bool _isPaused = false;

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

        if (Time.timeScale != 1f)
        {
            Time.timeScale = 1f;
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (_crosshair != null)
        {
            _crosshair.Show();
        }

        _isPaused = false;

        if (_pausePanel != null)
        {
            _pausePanel.SetActive(false);
        }
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) == true)
        {
            if (_resultPanel != null && _resultPanel.activeSelf == true)
            {
                return;
            }

            if (_isPaused == false)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }
        }
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
        _isPaused = false;

        if (_pausePanel != null)
        {
            _pausePanel.SetActive(false);
        }

        if (_crosshair != null)
        {
            _crosshair.Hide();
        }

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

        _isPaused = false;
        Time.timeScale = 1f;

        if (_crosshair != null)
        {
            _crosshair.Show();
        }

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
        _isPaused = false;
        Time.timeScale = 1f;

        if (_crosshair != null)
        {
            _crosshair.Show();
        }

        int current = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(current);
    }

    public void OnMainMenuClicked()
    {
        if (_crosshair != null)
        {
            _crosshair.Hide();
        }

        _isPaused = false;
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


    private void PauseGame()
    {
        _isPaused = true;
        Time.timeScale = 0f;

        if (_crosshair != null)
        {
            _crosshair.Hide();
        }

        if (_pausePanel != null)
        {
            _pausePanel.SetActive(true);
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void ResumeGame()
    {
        _isPaused = false;
        Time.timeScale = 1f;

        if (_pausePanel != null)
        {
            _pausePanel.SetActive(false);
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (_crosshair != null)
        {
            _crosshair.Show();
        }
    }

    public void OnResumeClicked()
    {
        if (_isPaused == false)
        {
            return;
        }

        ResumeGame();
    }
}
