using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("HP")]
    [SerializeField] private int _maxHp = 100;
    [SerializeField] private bool _startAtFullHp = true;

    [Header("Events")]
    [SerializeField] private UnityEvent<int> _onDamaged;
    [SerializeField] private UnityEvent _onDied;

    [Header("Animator")]
    [SerializeField] private Animator _anim;

    private int _currentHp;
    private bool _isDead;

    private PlayerController _playerController;
    private PlayerAttackMelee _playerAttackMelee;

    public int CurrentHP { get { return _currentHp; } }
    public int MaxHP { get { return _maxHp; } }
    public bool IsDead { get { return _isDead; } }


    void Awake()
    {
        if (_startAtFullHp == true)
        {
            _currentHp = _maxHp;
        }
        else
        {

            _currentHp = Mathf.Clamp(_currentHp, 0, _maxHp);
        }

        _playerController = GetComponent<PlayerController>();
        _playerAttackMelee = GetComponent<PlayerAttackMelee>();
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
        _currentHp = Mathf.Clamp(_currentHp - amount, 0, _maxHp);
        int dmgTaken = hpBeforeDmg - _currentHp;

        if (dmgTaken > 0)
        {
            _onDamaged?.Invoke(dmgTaken);
        }

        if (_currentHp <= 0)
        {
            _isDead = true;

            if (_anim != null)
            {
                _anim.SetBool("Dead", true);
            }

            if (_playerController != null)
            {
                _playerController.enabled = false;
            }
            if (_playerAttackMelee != null)
            {
                _playerAttackMelee.enabled = false;
            }

            _onDied?.Invoke();
        }
    }

    public void ResetToFull()
    {
        _currentHp = _maxHp;
        _isDead = false;

        if (_anim != null)
        {
            _anim.SetBool("Dead", false);
        }

        if (_playerController != null)
        {
            _playerController.enabled = true;
        }

        if (_playerAttackMelee != null)
        {
            _playerAttackMelee.enabled = true;
        }

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
