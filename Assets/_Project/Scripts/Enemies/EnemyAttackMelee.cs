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
    [SerializeField] private float _windup = 0.2f;
    [SerializeField] private float _hitRadius = 0.75f;

    private Transform _target;

    public void SetupRuntime(int damage, float cooldown, float windup, float hitRadius, float angleTol, Transform target)
    {
        _damage = damage;
        _windup = windup;
        _hitRadius = hitRadius;
        _target = target;
    }

    public void PerformAttackWithWindupFallback(float windup)
    {
        if (windup < 0f)
        {
            windup = 0f;
        }
        StartCoroutine(Co_DelayedHit(windup));
    }

    private IEnumerator Co_DelayedHit(float windup)
    {
        yield return new WaitForSeconds(windup);
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
