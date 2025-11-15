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

    [Header("Intention Ray (Crosshair Only)")]
    [SerializeField] private RectTransform _crosshairRect;
    [SerializeField] private Camera _uiCamera;
    [SerializeField] private float _intentMaxDistance = 3.5f;
    [SerializeField] private float _intentSphereRadius = 0.15f;
    [SerializeField] private bool _requireIntentToAttack = false;

    private float _nextAttackTime;
    private bool _isSwinging;
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

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) == true)
        {
            if (Time.time >= _nextAttackTime)
            {
                if (_isSwinging == false)
                {
                    bool canStart = EvaluateIntent();
                    if (_requireIntentToAttack == true && canStart == false)
                    {
                        return;
                    }

                    _nextAttackTime = Time.time + _attackCooldown;
                    StartCoroutine(Co_Swing());
                }
            }
        }
    }

    private bool EvaluateIntent()
    {
        if (_crosshairRect == null)
        {
            return true;
        }

        Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(_uiCamera, _crosshairRect.position);
        Ray ray = Camera.main.ScreenPointToRay(screenPos);

        RaycastHit hit;
        bool hasHit = Physics.SphereCast(
            ray,
            _intentSphereRadius,
            out hit,
            _intentMaxDistance,
            _effectiveTargetMask,
            QueryTriggerInteraction.Collide
        );

        return hasHit;
    }

    private IEnumerator Co_Swing()
    {
        _isSwinging = true;

        if (_anim != null)
        {
            _anim.SetTrigger("Attack");
        }

        if (_swingDuration < 0f)
        {
            _swingDuration = 0f;
        }

        yield return new WaitForSeconds(_swingDuration);

        _isSwinging = false;
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
