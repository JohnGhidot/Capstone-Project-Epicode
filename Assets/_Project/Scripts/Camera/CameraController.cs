using UnityEngine;
using Cinemachine;

[DisallowMultipleComponent]
public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _vcam;
    private CinemachinePOV _pov;

    [SerializeField] private string _mouseX = "Mouse X";
    [SerializeField] private string _mouseY = "Mouse Y";

    [SerializeField] private float _sensitivityX = 250f;
    [SerializeField] private float _sensitivityY = 120f;
    [SerializeField] private bool _invertY = false;

    [SerializeField] private float _minVertical = -10f;
    [SerializeField] private float _maxVertical = 35f;

    [SerializeField] private bool _lockCursor = true;

    private void Awake()
    {
        if (_vcam == null) { _vcam = GetComponent<CinemachineVirtualCamera>(); }
        if (_vcam != null) { _pov = _vcam.GetCinemachineComponent<CinemachinePOV>(); }

        if (_lockCursor == true)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (_pov != null)
        {
            _pov.m_HorizontalAxis.m_InputAxisName = "";
            _pov.m_VerticalAxis.m_InputAxisName = "";
            _pov.m_VerticalAxis.m_MinValue = _minVertical;
            _pov.m_VerticalAxis.m_MaxValue = _maxVertical;
            _pov.m_VerticalAxis.Value = Mathf.Clamp(_pov.m_VerticalAxis.Value, _minVertical, _maxVertical);
        }
    }

    private void OnEnable()
    {
        if (_vcam == null) { _vcam = GetComponent<CinemachineVirtualCamera>(); }
        if (_vcam != null) { _vcam.PreviousStateIsValid = false; }

        if (_pov == null && _vcam != null) { _pov = _vcam.GetCinemachineComponent<CinemachinePOV>(); }
        if (_pov != null)
        {
            _pov.m_VerticalAxis.Value = Mathf.Clamp(5f, _minVertical, _maxVertical);
        }
    }

    private void OnDisable()
    {
        if (_lockCursor == true)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void LateUpdate()
    {
        if (_pov == null) { return; }

        float deltaX = Input.GetAxisRaw(_mouseX) * _sensitivityX * Time.deltaTime;
        float deltaY = Input.GetAxisRaw(_mouseY) * _sensitivityY * Time.deltaTime;
        if (_invertY == false) { deltaY = -deltaY; }

        _pov.m_HorizontalAxis.m_InputAxisValue = deltaX;
        _pov.m_VerticalAxis.m_InputAxisValue = deltaY;
    }
}

