using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class EnemyAttackMelee : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private int _damage = 10;
    [SerializeField] private float _windup = 0.3f;
    [SerializeField] private float _attackCooldown = 1.5f;
    [SerializeField] private float _swingDuration = 0.4f;

    [Header("Facing")]
    [SerializeField] private bool _faceDuringWindup = true;
    [SerializeField] private float _faceTurnSpeed = 900f;

    [Header("Weapon Hitbox")]
    [SerializeField] private WeaponHitbox _weaponHitbox;
    [SerializeField] private LayerMask _targetMaskForWeapon;

    private Transform _target;
    private float _nextAttackTime;

    public void SetupRuntime(int damage, float cooldown, float windup, float hitRadius, float angleTol, Transform target)
    {
        _damage = damage;
        _windup = windup;
        _attackCooldown = cooldown;
        _target = target;

        if (_weaponHitbox != null)
        {
            _weaponHitbox.Setup(transform, _damage, _targetMaskForWeapon);
        }
    }

    public void PerformAttackWithWindupFallback(float windup)
    {
        if (Time.time < _nextAttackTime) { return; }
        if (_target == null) { return; }

        _nextAttackTime = Time.time + _attackCooldown;
        StartCoroutine(Co_SwingAfterWindup(windup));
    }

    private IEnumerator Co_SwingAfterWindup(float windup)
    {
        float t = 0f;
        while (t < windup)
        {
            if (_faceDuringWindup == true) { FaceTarget(Time.deltaTime); }
            t += Time.deltaTime;
            yield return null;
        }

        if (_weaponHitbox != null) { _weaponHitbox.BeginSwing(); }
        yield return new WaitForSeconds(_swingDuration);
        if (_weaponHitbox != null) { _weaponHitbox.EndSwing(); }
    }

    private void FaceTarget(float dt)
    {
        if (_target == null) { return; }

        Vector3 dir = _target.position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f) { return; }

        Quaternion look = Quaternion.LookRotation(dir, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, look, _faceTurnSpeed * dt);
    }
}
