using UnityEngine.UI;
using UnityEngine;



public enum GameMode { Duel = 0, Joust = 1 }

[DisallowMultipleComponent]
public class MainMenuBackground : MonoBehaviour
{
    public const string PREF_KEY_MODE = "SelectGameMode";


    [Header("References")]
    [SerializeField] private Image _backGroundImage;
    [SerializeField] private Sprite _duelBackground;
    [SerializeField] private Sprite _joustBackground;

    [Header("Defaluts")]
    [SerializeField] private GameMode _defaultMode = GameMode.Duel;


    private GameMode _currentMode = GameMode.Duel;

    private void Awake()
    {
        if (PlayerPrefs.HasKey(PREF_KEY_MODE) == true)
        {
            int saved = PlayerPrefs.GetInt(PREF_KEY_MODE);

            if (saved == (int)(GameMode.Joust))
            {
                _currentMode = GameMode.Joust;
            }
            else
            {
                _currentMode = GameMode.Duel;
            }
        }
        else
        {
            _currentMode = _defaultMode;
        }

        ApplyBackGroundForMode(_currentMode);
    }

    public void SetModeToDuel()
    {
        SetMode(GameMode.Duel);
    }

    public void SetModeToJoust()
    {
        SetMode(GameMode.Joust);
    }


    public void SetMode(GameMode mode)
    {
        _currentMode = mode;

        PlayerPrefs.SetInt(PREF_KEY_MODE, (int)_currentMode);
        PlayerPrefs.Save();

        ApplyBackGroundForMode(_currentMode);
    }

    private void ApplyBackGroundForMode(GameMode mode)
    {
        if (_backGroundImage == null)
        {
            return;
        }

        if (mode == GameMode.Joust)
        {
            if (_joustBackground != null)
            {
                _backGroundImage.sprite = _joustBackground;
            }
        }
        else
        {
            if (_duelBackground != null)
            {
                _backGroundImage.sprite = _duelBackground;
            }
        }


        _backGroundImage.rectTransform.anchorMin = Vector2.zero;
        _backGroundImage.rectTransform.anchorMax = Vector2.one;

        _backGroundImage.rectTransform.offsetMin = Vector2.zero;
        _backGroundImage.rectTransform.offsetMax = Vector2.zero;

    }
    public GameMode GetCurrentMode()
    {
        return _currentMode;
    }
}
