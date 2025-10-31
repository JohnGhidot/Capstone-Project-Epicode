using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
public class PlayerAttackMelee : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _hitOrigin;
    [SerializeField] private Animator _anim;

    [Header("Attack Settings")]
    [SerializeField] private int _damage = 20;
    [SerializeField] private float _attackCooldown = 0.9f;

    [Header("Weapon Hitbox")]
    [SerializeField] private WeaponHitbox _weaponHitbox;
    [SerializeField] private LayerMask _targetMaskForWeapon;
    [SerializeField] private float _swingDuration = 0.4f;

    private float _nextAttackTime;
    private bool _isSwinging;

    private void Awake()
    {
        if (_weaponHitbox != null)
        {
            _weaponHitbox.Setup(transform, _damage, _targetMaskForWeapon);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) == true)
        {
            if (Time.time >= _nextAttackTime && _isSwinging == false)
            {
                _nextAttackTime = Time.time + _attackCooldown;
                StartCoroutine(Co_Swing());
            }
        }
    }

    private IEnumerator Co_Swing()
    {
        _isSwinging = true;

        if (_anim != null)
        {
            _anim.SetTrigger("Attack");
        }

        if (_weaponHitbox != null)
        {
            _weaponHitbox.BeginSwing();
        }

        yield return new WaitForSeconds(_swingDuration);

        if (_weaponHitbox != null)
        {
            _weaponHitbox.EndSwing();
        }

        _isSwinging = false;
    }
}
