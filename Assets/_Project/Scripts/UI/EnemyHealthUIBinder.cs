using UnityEngine;

[RequireComponent(typeof(EnemyHealth))]
[DisallowMultipleComponent]
public class EnemyHealthUIBinder : MonoBehaviour
{
    [SerializeField] private HealthBarDriver _driver;
    [SerializeField] private UIManager _ui;

    private EnemyHealth _health;

    private void Awake()
    {
        _health = GetComponent<EnemyHealth>();
    }

    private void OnEnable()
    {
        if (_health != null)
        {
            _health.OnDamaged += HandleDamaged;
            _health.OnDied += HandleDied;
        }
    }

    private void OnDisable()
    {
        if (_health != null)
        {
            _health.OnDamaged -= HandleDamaged;
            _health.OnDied -= HandleDied;
        }
    }

    private void Start()
    {
        if (_health != null && _driver != null)
        {
            _driver.SetBoth(_health.CurrentHP, _health.MaxHP);
        }
    }

    private void HandleDamaged(int current, int max, GameObject source)
    {
        if (_driver != null)
        {
            _driver.SetBoth(current, max);
        }
    }

    private void HandleDied(GameObject source)
    {
        if (_ui != null)
        {
            _ui.OnEnemyDefeated();
        }
    }
}
