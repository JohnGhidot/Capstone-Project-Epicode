using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class EnemyAttackMelee : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _hitOrigin;
    [SerializeField] private LayerMask _hitMask;

    [Header("Runtime Attack Parameters")]
    [SerializeField] private int _damage = 10;
    [SerializeField] private float _cooldown = 1.2f;
    [SerializeField] private float _windup = 0.2f;
    [SerializeField] private float _hitRadius = 0.75f;
    [SerializeField] private float _attackAngleTolerance = 60f;

    private float _nextAttackTime = 0f;
    private Animator _anim;
    private Transform _target;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
    }

    public void SetupRuntime(int damage, float cooldown, float windup, float hitRadius, float angleTol, Transform target)
    {
        _damage = damage;
        _cooldown = cooldown;
        _windup = windup;
        _hitRadius = hitRadius;
        _attackAngleTolerance = angleTol;
        _target = target;
    }

    private bool IsReadyToAttacK()
    {
        if (Time.time >= _nextAttackTime)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsFacingTarget(Transform t, float toleranceDeg)
    {
        if (t == null)
        {
            return false;
        }
        Vector3 toTarget = t.position - transform.position;
        toTarget.y = 0f;

        if (toTarget.sqrMagnitude < 0.0001f)
        {
            return true;
        }

        return Vector3.Angle(transform.forward, toTarget.normalized) <= toleranceDeg;
    }

    public void TryAttack()
    {
        if (!IsReadyToAttacK() || _target == null || !IsFacingTarget(_target, _attackAngleTolerance))
        {
            return;
        }

        _nextAttackTime = Time.time + _cooldown;

        if (_anim != null)
        {
            _anim.SetTrigger("Attack");
        }
        else
        {
            StartCoroutine(Co_DelayedHit());
        }
    }

    private IEnumerator Co_DelayedHit()
    {
        yield return new WaitForSeconds(_windup);
        DoMeleeHit();
    }

    private void AnimationEvent_AttackHit()
    {
        DoMeleeHit();
    }

    private void DoMeleeHit()
    {
        if (_target == null)
        {
            return;
        }

        Vector3 center;
        if (_hitOrigin != null)
        {
            center = _hitOrigin.position;
        }
        else
        {
            center = transform.position;
        }

        Collider[] hits = Physics.OverlapSphere(center, _hitRadius, _hitMask, QueryTriggerInteraction.Collide);

        int hitsLength = hits.Length;
        if (hitsLength == 0)
        {
            return;
        }

        Transform myRoot = transform.root;

        for (int i = 0; i < hitsLength; i = i + 1)
        {
            Collider c = hits[i];

            if (c == null)
            {
                continue;
            }

            Transform hitRoot = c.transform.root;

            bool isSelf = false;
            if (hitRoot == myRoot)
            {
                isSelf = true;
            }

            if (isSelf == true)
            {
                continue;
            }

            IDamageable dmg = c.GetComponentInParent<IDamageable>();

            bool hasDamageable = false;
            if (dmg != null)
            {
                hasDamageable = true;
            }

            if (hasDamageable == true)
            {
                dmg.TakeDamage(_damage, gameObject);
                break;
            }
        }
    }
}
