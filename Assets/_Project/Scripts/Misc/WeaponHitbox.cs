using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class WeaponHitbox : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Collider _trigger;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Transform _ownerRoot;
    [SerializeField] private Animator _ownerAnim;

    [Header("Hit Settings")]
    [SerializeField] private int _damage = 20;
    [SerializeField] private LayerMask _targetMask;
    [SerializeField] private float _hitStopDuration = 0.08f;
    [SerializeField] private float _resumeBlendTime = 0.06f;

    private bool _isArmed;
    private readonly HashSet<IDamageable> _hitThisSwing = new HashSet<IDamageable>();
    private bool _hitStopActive = false;

    private void Reset()
    {
        _trigger = GetComponent<Collider>();
        _rb = GetComponent<Rigidbody>();
        if (_trigger != null) { _trigger.isTrigger = true; }
        if (_rb != null) { _rb.isKinematic = true; _rb.useGravity = false; }
    }

    private void Awake()
    {
        if (_trigger == null) { _trigger = GetComponent<Collider>(); }
        if (_trigger != null) { _trigger.isTrigger = true; }

        if (_rb == null) { _rb = GetComponent<Rigidbody>(); }
        if (_rb != null) { _rb.isKinematic = true; _rb.useGravity = false; }

        if (_ownerAnim == null)
        {
            _ownerAnim = GetComponentInParent<Animator>();
        }
    }

    public void Setup(Transform ownerRoot, int damage, LayerMask targetMask)
    {
        _ownerRoot = ownerRoot;
        _damage = damage;
        _targetMask = targetMask;
    }

    public void BeginSwing()
    {
        _isArmed = true;
        _hitThisSwing.Clear();
    }

    public void EndSwing()
    {
        _isArmed = false;
        _hitThisSwing.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        TryHit(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryHit(other);
    }

    private void TryHit(Collider other)
    {
        if (_isArmed == false) { return; }
        if (other == null) { return; }

        int otherBit = 1 << other.gameObject.layer;
        if ((_targetMask.value & otherBit) == 0) { return; }

        if (_ownerRoot != null)
        {
            if (other.transform.IsChildOf(_ownerRoot) == true) { return; }
        }

        IDamageable dmg = other.GetComponentInParent<IDamageable>();
        if (dmg == null) { return; }
        if (_hitThisSwing.Contains(dmg) == true) { return; }

        _hitThisSwing.Add(dmg);

        if (_ownerAnim != null)
        {
            StartCoroutine(HitStopRoutine(_ownerAnim));
        }

        GameObject source = (_ownerRoot != null) ? _ownerRoot.gameObject : gameObject;
        dmg.TakeDamage(_damage, source);
    }

    private IEnumerator HitStopRoutine(Animator attackerAnim)
    {
        if (attackerAnim == null) { yield break; }
        if (_hitStopActive == true) { yield break; }

        _hitStopActive = true;

        float prevSpeed = attackerAnim.speed;
        attackerAnim.speed = 0f;

        float t = 0f;
        while (t < _hitStopDuration)
        {
            t += Time.unscaledDeltaTime;
            yield return null;
        }

        float blend = 0f;
        while (blend < _resumeBlendTime)
        {
            blend += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(blend / _resumeBlendTime);
            attackerAnim.speed = Mathf.Lerp(0f, prevSpeed, k);
            yield return null;
        }

        attackerAnim.speed = prevSpeed;
        _hitStopActive = false;
    }
}
