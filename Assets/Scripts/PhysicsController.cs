using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PhysicsController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float walkThreshold = 0.1f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.15f;
    [SerializeField] private LayerMask groundLayers;

    [Header("Jump Forgiveness")]
    [SerializeField]
    private float coyoteTime = 0.2f;
    [SerializeField]
    private float jumpCooldown = 0.2f;

    [Header("Input Actions")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;

    [Header("Mobile Input")]
    [SerializeField] private Joystick moveJoystick;
    [SerializeField] private bool useJoystickInEditor = false;

    [SerializeField] private bool flipByScale = true;

    private Rigidbody2D _rigidbody;
    private Vector2 _moveInput;
    private bool _jumpRequested;
    private float _lastGroundedTime = float.NegativeInfinity;
    private bool _isGrounded;
    private float _lastJumpTime = float.NegativeInfinity;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        if (moveAction != null)
        {
            moveAction.action.performed += OnMove;
            moveAction.action.canceled += OnMove;
            moveAction.action.Enable();
        }

        if (jumpAction != null)
        {
            jumpAction.action.performed += OnJump;
            jumpAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (moveAction != null)
        {
            moveAction.action.performed -= OnMove;
            moveAction.action.canceled -= OnMove;
            moveAction.action.Disable();
        }

        if (jumpAction != null)
        {
            jumpAction.action.performed -= OnJump;
            jumpAction.action.Disable();
        }
    }


    private void FixedUpdate()
    {
        UpdateGroundStatus();
        MoveHorizontal();
        HandleJump();
    }

    private void UpdateGroundStatus()
    {
        _isGrounded = IsGrounded();

        if (_isGrounded)
        {
            _lastGroundedTime = Time.time;
        }
    }

    private void MoveHorizontal()
    {
        Vector2 moveInput = GetMoveInput();
        var velocity = _rigidbody.linearVelocity;
        velocity.x = moveInput.x * moveSpeed;
        _rigidbody.linearVelocity = velocity;

        if (flipByScale && Mathf.Abs(moveInput.x) > 0.01f)
        {
            var scale = transform.localScale;
            scale.x = Mathf.Sign(moveInput.x) * Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }

    private void HandleJump()
    {
        if (!_jumpRequested)
            return;

        if (!CanUseCoyoteTime())
        {
            _jumpRequested = false;
            return;
        }

        if (!IsJumpOffCooldown())
        {
            _jumpRequested = false;
            return;
        }

        _rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        _jumpRequested = false;
        _lastJumpTime = Time.time;
    }

    public bool IsGrounded()
    {
        if (groundCheck == null)
            return false;

        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayers);
    }

    private bool CanUseCoyoteTime()
    {
        return _isGrounded || Time.time - _lastGroundedTime <= coyoteTime;
    }

    private bool IsJumpOffCooldown()
    {
        return Time.time - _lastJumpTime >= jumpCooldown;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && CanUseCoyoteTime())
        {
            _jumpRequested = true;
        }
    }

    public void RequestJump()
    {
        _jumpRequested = true;
    }

    private Vector2 GetMoveInput()
    {
        if (moveJoystick == null)
            return _moveInput;

        bool allowJoystick = Application.isMobilePlatform || useJoystickInEditor;
        if (!allowJoystick)
            return _moveInput;

        Vector2 joystickInput = moveJoystick.Direction;
        if (joystickInput.sqrMagnitude > 0.0001f)
            return joystickInput;

        return _moveInput;
    }

    void OnDrawGizmosSelected()
    {
        if(groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
