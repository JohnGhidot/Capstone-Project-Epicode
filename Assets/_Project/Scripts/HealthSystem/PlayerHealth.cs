using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("HP")]
    [SerializeField] private int _maxhHp = 100;
    [SerializeField] private bool _startAtFullHp = true;

    [Header("Events")]
    [SerializeField] private UnityEvent<int> _onDamaged;
    [SerializeField] private UnityEvent _onDied;

    private int _currentHp;
    private bool _isDead;

    public int CurrentHP { get { return _currentHp; } }
    public int MaxhHP { get { return _maxhHp; } }
    public bool IsDead { get { return _isDead; } }


    void Awake()
    {
        if (_startAtFullHp == true)
        {
            _currentHp = _maxhHp;
        }
        else
        {

            _currentHp = Mathf.Clamp(_currentHp, 0, _maxhHp);
        }
    }

    public void TakeDamage(int amount, GameObject source)
    {
        if (amount <= 0)
        {
            return;
        }

        if (_isDead == true)
        {
            return;
        }

        int hpBeforeDmg = _currentHp;
        _currentHp = Mathf.Clamp(_currentHp - amount, 0, _maxhHp);
        int dmgTaken = hpBeforeDmg - _currentHp;

        if (dmgTaken > 0)
        {
            _onDamaged?.Invoke(dmgTaken);
        }

        if (_currentHp <= 0)
        {
            _isDead = true;
            _onDied?.Invoke();
        }
    }

    public void ResetToFull()
    {
        _currentHp = _maxhHp;
        _isDead = false;
    }

    public void Kill()
    {
        if (_isDead == true)
        {
            return;
        }

        TakeDamage(_currentHp, gameObject);

    }

}
