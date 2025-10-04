using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private enum MoveReference { World, Orientation }

    [Header("References")]
    [SerializeField] private CharacterController _controller;
    [SerializeField] private Transform _orientation;

    [Header("Movement")]
    [SerializeField] private MoveReference _moveReference = MoveReference.Orientation;
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _rotationSpeed = 720f;

    [Header("Gravity Force")]
    [SerializeField] private float _gravityStickForce = 2f;


    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        
    }

    private void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector2 input = new Vector2(h, v);

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

        Vector3 horizontal = moveDir.normalized * _moveSpeed;
        Vector3 vertical = Vector3.down * _gravityStickForce;

        _controller.Move((horizontal + vertical) * Time.deltaTime);
    }
}
