using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class EnemyMoverKinematic : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _target;
    [SerializeField] private Animator _anim;

    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 3.5f;
    [SerializeField] private float _turnSpeed = 540f;
    [SerializeField] private float _stopDistance = 1.6f;

    [Header("Obstacles Avoidance")]
    [SerializeField] private LayerMask _obstacleMask;
    [SerializeField] private float _rayDistance = 1.2f;
    [SerializeField] private float _sideAngle = 25f;
    [SerializeField] private float _avoidStrength = 0.8f;

    private CharacterController _characterController;

    private float _animSpeed;
    private float _animSpeedVel;
    [SerializeField] private float _animSpeedDamp = 0.08f;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();

        if (_anim == null)
        {
            _anim = GetComponent<Animator>();
        }
    }

    public void SetTarget(Transform t)
    {
        _target = t;
    }

    public void Tick()
    {
        if (_target == null)
        {
            if (_anim != null)
            {
                _anim.SetFloat("Speed", 0f);
            }

            return;
        }

        Vector3 directionToTarget = _target.position - transform.position;
        directionToTarget.y = 0f;

        if (directionToTarget.sqrMagnitude <= _stopDistance * _stopDistance)
        {
            if (_anim != null)
            {
                _anim.SetFloat("Speed", 0f);
            }

            RotateTowards(_target.position);
            return;
        }

        Vector3 dir = directionToTarget.normalized;

        Vector3 fwd = transform.forward;
        Vector3 left = Quaternion.Euler(0f, -_sideAngle, 0f) * fwd;
        Vector3 right = Quaternion.Euler(0f, _sideAngle, 0f) * fwd;

        Vector3 avoid = Vector3.zero;

        if (Physics.Raycast(transform.position + Vector3.up, fwd, _rayDistance * 9f, _obstacleMask, QueryTriggerInteraction.Ignore))
        {
            avoid += -fwd;
        }

        if (Physics.Raycast(transform.position + Vector3.up, left, _rayDistance * 9f, _obstacleMask, QueryTriggerInteraction.Ignore))
        {
            avoid += right;
        }

        if (Physics.Raycast(transform.position + Vector3.up, right, _rayDistance * 9f, _obstacleMask, QueryTriggerInteraction.Ignore))
        {
            avoid += left;
        }

        if (avoid != Vector3.zero)
        {
            dir = Vector3.Lerp(dir, avoid.normalized, _avoidStrength).normalized;
        }

        Quaternion targetRotation = Quaternion.LookRotation(dir, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _turnSpeed * Time.deltaTime);

        Vector3 step = dir * _moveSpeed * Time.deltaTime;

        if (_characterController != null)
        {
            _characterController.Move(step);
        }
        else
        {
            transform.position += step;
        }

        float currentSpeed;

        if (_characterController != null)
        {
            currentSpeed = _characterController.velocity.magnitude;
        }
        else
        {
            float dt = Time.deltaTime;

            if (dt > 0f)
            {
                currentSpeed = step.magnitude / dt;
            }
            else
            {
                currentSpeed = 0f;
            }
        }

        _animSpeed = Mathf.SmoothDamp(_animSpeed, currentSpeed, ref _animSpeedVel, _animSpeedDamp);

        if (_anim != null)
        {
            _anim.SetFloat("Speed", _animSpeed);
        }
    }

    private void RotateTowards(Vector3 worldPos)
    {
        Vector3 dir = worldPos - transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.0001f)
        {
            return;
        }

        Quaternion rotation = Quaternion.LookRotation(dir, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, _turnSpeed * Time.deltaTime);
    }

    public void SetMoveParams(float moveSpeed, float turnSpeed, float stopDistance)
    {
        _moveSpeed = moveSpeed;
        _turnSpeed = turnSpeed;
        _stopDistance = stopDistance;
    }
}
