using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerAttackMelee : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _hitOrigin;
    [SerializeField] private Animator _anim;

    [Header("Attack Settings")]
    [SerializeField] private int _damage = 20;
    [SerializeField] private float _attackCooldown = 0.9f;
    [SerializeField] private float _hitRadius = 0.9f;
    [SerializeField] private LayerMask _hitMask;

    [Header("Parry Settings")]
    [SerializeField] private float _parryWindow = 0.3f;
    [SerializeField] private float _parryCooldown = 0.9f;

    private float _nextAttackTime;
    private float _nextParryTime;
    private float _parryActiveUntil;
    private bool _parryFlag;

    private readonly HashSet<IDamageable> _hitThisSwing = new HashSet<IDamageable>();

    public bool IsParryActive
    {
        get
        {
            return _parryFlag == true && Time.time <= _parryActiveUntil;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) == true)
        {
            if (Time.time >= _nextAttackTime)
            {
                _nextAttackTime = Time.time + _attackCooldown;
                //if (_anim != null)
                //{
                //    _anim.SetTrigger("Attack");
                //}
            }
        }

        if (Input.GetMouseButtonDown(1) == true)
        {
            if (Time.time >= _nextParryTime)
            {
                _nextParryTime = Time.time + _parryCooldown;
                _parryFlag = true;
                _parryActiveUntil = Time.time + _parryWindow;

                if (_anim != null)
                {
                    _anim.SetTrigger("Parry");
                }
            }
        }

        if (_parryFlag == true && Time.time > _parryActiveUntil)
        {
            _parryFlag = false;
        }

    }

    public void OnAttackStart()
    {
        _hitThisSwing.Clear();
    }

    public void DoMeleeHit()
    {
        Vector3 center;
        if (_hitOrigin != null)
        {
            center = _hitOrigin.position;
        }
        else
        {
            Vector3 forwardOffset = transform.forward * 0.9f;
            Vector3 upwardOffset = Vector3.up * 1.2f;
            center = transform.position + forwardOffset + upwardOffset;

        }

        Collider[] hits = Physics.OverlapSphere(center, _hitRadius, _hitMask, QueryTriggerInteraction.Collide);

        if (hits == null || hits.Length == 0)
        {
            return;
        }

        for (int i = 0; i < hits.Length; i++)
        {
            Collider currentCollider = hits[i];

            if (currentCollider == null)
            {
                continue;
            }

            IDamageable dmg = currentCollider.GetComponentInParent<IDamageable>();

            if (dmg == null)
            {
                continue;
            }

            if (_hitThisSwing.Contains(dmg) == true)
            {
                continue;
            }

            dmg.TakeDamage(_damage, gameObject);

            _hitThisSwing.Add(dmg);
        }
    }

    public void OnAttackEnd()
    {
        _hitThisSwing.Clear();
    }

    private void OnDrawGizmosSelected()
    {
        if (_hitOrigin != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_hitOrigin.position, _hitRadius);
        }
    }
}