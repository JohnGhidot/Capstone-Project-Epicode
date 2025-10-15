using UnityEngine;
using Cinemachine;

[DisallowMultipleComponent]
public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _vcam;
    private CinemachinePOV _pov;

    [SerializeField] private CinemachineVirtualCamera _idleVcam;
    private CinemachineOrbitalTransposer _idleOrb;
    private CinemachinePOV _idlePov;

    [Header("Input Axes (Look only)")]
    [SerializeField] private string _mouseX = "Mouse X";
    [SerializeField] private string _mouseY = "Mouse Y";

    [Header("Idle Trigger Source")]
    [SerializeField] private bool _usePlayerControllerForIdle = true;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private float _movementDeadzone = 0.08f;

    [Header("POV (Movement Cam)")]
    [SerializeField] private float _sensitivityX = 250f;
    [SerializeField] private float _sensitivityY = 120f;
    [SerializeField] private bool _invertY = false;
    [SerializeField] private float _minVertical = -10f;
    [SerializeField] private float _maxVertical = 35f;

    [Header("Idle Settings")]
    [SerializeField] private float _idleDelaySeconds = 1.25f;

    [Header("Misc")]
    [SerializeField] private bool _lockCursor = true;

    [Header("Priorities (higher = active)")]
    [SerializeField] private int _movementCamPriority = 20;
    [SerializeField] private int _idleCamPriority = 10;

    private float _lastActivityTime = 0f;
    private bool _isIdle = false;

    private float _smoothedLookX;
    private float _smoothedLookY;

    private void Awake()
    {
        if (_vcam != null)
        {
            _pov = _vcam.GetCinemachineComponent<CinemachinePOV>();
        }

        if (_idleVcam != null)
        {
            _idleOrb = _idleVcam.GetCinemachineComponent<CinemachineOrbitalTransposer>();
            _idlePov = _idleVcam.GetCinemachineComponent<CinemachinePOV>();
        }

        if (_pov != null)
        {
            _pov.m_HorizontalAxis.m_InputAxisName = string.Empty;
            _pov.m_VerticalAxis.m_InputAxisName = string.Empty;
        }
        if (_idlePov != null)
        {
            _idlePov.m_HorizontalAxis.m_InputAxisName = string.Empty;
            _idlePov.m_VerticalAxis.m_InputAxisName = string.Empty;
        }
        if (_idleOrb != null)
        {
            _idleOrb.m_XAxis.m_InputAxisName = string.Empty;
        }

        if (_lockCursor == true)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        SetMovementCameraActive(true);
        _lastActivityTime = Time.time;
    }

    private void LateUpdate()
    {
        float rawX = Input.GetAxisRaw(_mouseX);
        float rawY = Input.GetAxisRaw(_mouseY);

        _smoothedLookX = Mathf.Lerp(_smoothedLookX, rawX, Time.deltaTime * 10f);
        _smoothedLookY = Mathf.Lerp(_smoothedLookY, rawY, Time.deltaTime * 10f);

        float lookX = _smoothedLookX;
        float lookY = _smoothedLookY;

        bool hasMove = false;

        if (_usePlayerControllerForIdle == true && _playerController != null)
        {
            hasMove = _playerController.IsMoving;
        }
        else
        {
            float mx = Input.GetAxisRaw("Horizontal");
            float my = Input.GetAxisRaw("Vertical");
            Vector2 raw = new Vector2(mx, my);

            if (raw.magnitude < _movementDeadzone)
            {
                raw = Vector2.zero;
            }
            else
            {
                raw = raw.normalized;
            }

            hasMove = (raw.sqrMagnitude > 0f);
        }

        if (hasMove == true)
        {
            _lastActivityTime = Time.time;
            if (_isIdle == true) { SetMovementCameraActive(true); }
        }
        else
        {
            if (_isIdle == false)
            {
                if ((Time.time - _lastActivityTime) >= _idleDelaySeconds)
                {
                    SetMovementCameraActive(false);
                }
            }
        }

        if (_isIdle == false)
        {
            if (_pov != null)
            {
                _pov.m_HorizontalAxis.Value += lookX * _sensitivityX * Time.deltaTime;

                float deltaY = lookY * _sensitivityY * Time.deltaTime;
                if (_invertY == false) { deltaY = -deltaY; }

                float newPitch = _pov.m_VerticalAxis.Value + deltaY;
                newPitch = Mathf.Clamp(newPitch, _minVertical, _maxVertical);
                if (_pov.m_VerticalAxis.Value != newPitch) { _pov.m_VerticalAxis.Value = newPitch; }
            }
        }
        else
        {
            if (_idleOrb != null)
            {
                _idleOrb.m_XAxis.Value += lookX * _sensitivityX * Time.deltaTime;
            }
            if (_idleVcam != null)
            {
                CinemachineOrbitalTransposer transposer = _idleVcam.GetCinemachineComponent<CinemachineOrbitalTransposer>();
                if (transposer != null)
                {
                    float newY = transposer.m_FollowOffset.y + lookY * 0.05f;
                    newY = Mathf.Clamp(newY, 0.5f, 3f);
                    transposer.m_FollowOffset.y = newY;
                }
            }
        }
    }

    private void SetMovementCameraActive(bool active)
    {
        if (_vcam != null)
        {
            _vcam.Priority = (active == true) ? _movementCamPriority : _idleCamPriority;
        }

        if (_idleVcam != null)
        {
            _idleVcam.Priority = (active == true) ? _idleCamPriority : _movementCamPriority;
        }

        _isIdle = (active == false);
    }
}


