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

    [Header("Targeting/Gating")]
    [SerializeField] private float _attackRange = 2.2f;
    [SerializeField] private float _attackMaxAngleDeg = 60f;

    private Transform _target;
    private float _nextAttackTime;
    private LayerMask _effectiveTargetMask;

    private void Awake()
    {
        LayerMask maskToUse = _targetMaskForWeapon;
        if (maskToUse.value == 0)
        {
            int hurtboxLayer = LayerMask.NameToLayer("Hurtbox");
            if (hurtboxLayer >= 0)
            {
                maskToUse = 1 << hurtboxLayer;
            }
        }
        _effectiveTargetMask = maskToUse;

        if (_weaponHitbox != null)
        {
            _weaponHitbox.Setup(transform, _damage, maskToUse);
        }
    }

    public void SetupRuntime(int damage, float cooldown, float windup, float hitRadius, float angleTol, Transform target)
    {
        _damage = damage;
        _windup = windup;
        _attackCooldown = cooldown;
        _target = target;

        if (_weaponHitbox != null)
        {
            LayerMask maskToUse = _targetMaskForWeapon;
            if (maskToUse.value == 0)
            {
                int hurtboxLayer = LayerMask.NameToLayer("Hurtbox");
                if (hurtboxLayer >= 0)
                {
                    maskToUse = 1 << hurtboxLayer;
                }
            }

            _effectiveTargetMask = maskToUse;
            _weaponHitbox.Setup(transform, _damage, maskToUse);
        }
    }

    public void PerformAttackWithWindupFallback(float windup)
    {
        if (Time.time < _nextAttackTime)
        {
            return;
        }

        if (_target == null)
        {
            return;
        }

        if (IsTargetInGating() == false)
        {
            return;
        }

        _nextAttackTime = Time.time + _attackCooldown;
        StartCoroutine(Co_SwingAfterWindup(windup));
    }

    private IEnumerator Co_SwingAfterWindup(float windup)
    {
        float t = 0f;
        while (t < windup)
        {
            if (_faceDuringWindup == true)
            {
                FaceTarget(Time.deltaTime);
            }

            t += Time.deltaTime;
            yield return null;
        }

        if (_swingDuration < 0f)
        {
            _swingDuration = 0f;
        }

        yield return new WaitForSeconds(_swingDuration);
    }

    private void FaceTarget(float dt)
    {
        if (_target == null)
        {
            return;
        }

        Vector3 dir = _target.position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f)
        {
            return;
        }

        Quaternion look = Quaternion.LookRotation(dir, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, look, _faceTurnSpeed * dt);
    }

    private bool IsTargetInGating()
    {
        if (_target == null)
        {
            return false;
        }

        Vector3 toTarget = _target.position - transform.position;
        toTarget.y = 0f;

        float maxRange = _attackRange;
        if (maxRange < 0f)
        {
            maxRange = 0f;
        }

        if (toTarget.sqrMagnitude > (maxRange * maxRange))
        {
            return false;
        }

        float angle = Vector3.Angle(transform.forward, toTarget);
        if (angle > _attackMaxAngleDeg)
        {
            return false;
        }

        return true;
    }

    public void Anim_BeginSwing()
    {
        if (_weaponHitbox == null)
        {
            return;
        }

        _weaponHitbox.BeginSwing();
    }

    public void Anim_EndSwing()
    {
        if (_weaponHitbox == null)
        {
            return;
        }

        _weaponHitbox.EndSwing();
    }
}
