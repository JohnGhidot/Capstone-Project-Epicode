using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("Health")]
    [SerializeField] private int _maxHp = 100;

    [Header("Death")]
    [SerializeField] private bool _destroyOnDeath = true;
    [SerializeField] private float _destroyDelay = 2f;

    public event Action<int, int, GameObject> OnDamaged;
    public event Action<GameObject> OnDied;

    public int CurrentHP { get; private set; }
    public int MaxHP { get { return _maxHp; } }
    public bool IsDead { get; private set; }

    private Enemy _enemy;
    private Animator _anim;

    private void Awake()
    {
        if (_maxHp < 1) { _maxHp = 1; }

        CurrentHP = _maxHp;

        _enemy = GetComponent<Enemy>();
        _anim = GetComponent<Animator>();
    }

    public void TakeDamage(int amount, GameObject source)
    {
        if (IsDead == true) { return; }
        if (amount <= 0) { return; }

        int nextHP = CurrentHP - amount;
        if (nextHP < 0) { nextHP = 0; }
        CurrentHP = nextHP;

        if (OnDamaged != null)
        {
            OnDamaged.Invoke(CurrentHP, _maxHp, source);
        }

        if (CurrentHP > 0)
        {
            if (_anim != null)
            {
                _anim.SetTrigger("Hit");
            }
        }

        if (CurrentHP <= 0)
        {
            Die(source);
        }
    }

    public void Die(GameObject source)
    {
        if (IsDead == true) { return; }

        IsDead = true;

        if (_enemy != null)
        {
            _enemy.OnDeath();
        }

        EnemyAttackMelee attack = GetComponent<EnemyAttackMelee>();
        if (attack != null)
        {
            attack.enabled = false;
        }

        EnemyMoverKinematic mover = GetComponent<EnemyMoverKinematic>();
        if (mover != null)
        {
            mover.enabled = false;
        }

        Time.timeScale = 0f;

        if (OnDied != null)
        {
            OnDied.Invoke(source);
        }

        if (_destroyOnDeath == true)
        {
            float delay = _destroyDelay;
            if (delay < 0f) { delay = 0f; }
            Destroy(gameObject, delay);
        }
    }
}
