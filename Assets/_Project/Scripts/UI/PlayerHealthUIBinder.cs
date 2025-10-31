using UnityEngine;

[DisallowMultipleComponent]
public class PlayerHealthUIBinder : MonoBehaviour
{
    [SerializeField] private PlayerHealth _playerHealth;
    [SerializeField] private HealthBarDriver _driver;
    [SerializeField] private UIManager _ui;

    private void Start()
    {
        if (_playerHealth != null && _driver != null)
        {
            _driver.SetBoth(_playerHealth.CurrentHP, _playerHealth.MaxHP);
        }
    }

    public void OnPlayerDamaged(int damageTaken)
    {
        if (_playerHealth != null && _driver != null)
        {
            _driver.SetCurrent(_playerHealth.CurrentHP);
        }
    }

    public void OnPlayerDied()
    {
        if (_ui != null)
        {
            _ui.OnPlayerDefeated();
        }
    }
}

