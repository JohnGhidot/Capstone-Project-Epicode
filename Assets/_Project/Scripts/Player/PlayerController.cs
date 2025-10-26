using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private enum MoveReference { World, Orientation }

    [Header("References")]
    [SerializeField] private CharacterController _controller;
    [SerializeField] private Transform _orientation;
    [SerializeField] private Animator _anim;

    // [NEW] Riferimento all'UIManager per testare la UI
    [SerializeField] private UIManager _uiManager;

    [Header("Movement")]
    [SerializeField] private MoveReference _moveReference = MoveReference.Orientation;
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _rotationSpeed = 720f;

    [Header("Gravity")]
    [SerializeField] private float _gravityForce = -9.81f;

    [Header("Tuning")]
    [SerializeField] private float _terminalVelocity = -50f;
    [SerializeField] private float _inputDeadzone = 0.08f;

    private float _verticalSpeed;
    private bool _isGrounded;

    public bool IsMoving { get; private set; }
    public Vector2 LastMoveInput { get; private set; }

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        //--- TEST UI ---
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (_uiManager != null)
            {
                _uiManager.OnEnemyDefeated(); // Simula vittoria
            }
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            if (_uiManager != null)
            {
                _uiManager.OnPlayerDefeated(); // Simula sconfitta
            }
        }
        

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector2 input = new Vector2(h, v);

        if (input.magnitude < _inputDeadzone)
        {
            input = Vector2.zero;
        }
        else
        {
            input = input.normalized;
        }

        LastMoveInput = input;
        IsMoving = (input.sqrMagnitude > 0f);

        _isGrounded = _controller.isGrounded;

        Vector3 moveDir;
        if (_moveReference == MoveReference.Orientation && _orientation != null)
        {
            Vector3 fwd = _orientation.forward;
            fwd.y = 0f;
            fwd.Normalize();

            Vector3 right = _orientation.right;
            right.y = 0f;
            right.Normalize();

            moveDir = (fwd * input.y) + (right * input.x);
        }
        else
        {
            moveDir = new Vector3(input.x, 0f, input.y);
        }

        if (_moveReference == MoveReference.Orientation && _orientation != null)
        {
            if (input.sqrMagnitude > 0.0001f)
            {
                Vector3 face = _orientation.forward;
                face.y = 0f;
                face.Normalize();

                Quaternion target = Quaternion.LookRotation(face, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, target, _rotationSpeed * Time.deltaTime);
            }
        }
        else
        {
            if (moveDir.sqrMagnitude > 0.0001f)
            {
                Quaternion target = Quaternion.LookRotation(moveDir, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, target, _rotationSpeed * Time.deltaTime);
            }
        }

        if (_isGrounded == true && _verticalSpeed < 0f)
        {
            _verticalSpeed = -2f;
        }

        _verticalSpeed += _gravityForce * Time.deltaTime;

        if (_verticalSpeed < _terminalVelocity)
        {
            _verticalSpeed = _terminalVelocity;
        }

        Vector3 horizontal = moveDir.normalized * _moveSpeed;
        Vector3 vertical = new Vector3(0f, _verticalSpeed, 0f);

        _controller.Move((horizontal + vertical) * Time.deltaTime);

        if (_anim != null)
        {
            float speed01 = Mathf.Clamp01(horizontal.magnitude / Mathf.Max(0.01f, _moveSpeed));
            _anim.SetFloat("Speed", speed01);
        }
    }
}
