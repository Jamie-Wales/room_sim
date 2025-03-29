using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Input Settings")]
    private InputSystem _inputActions;
    private InputAction _moveAction;
    private InputAction _jumpAction;
    [SerializeField] private Transform head;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float sprintSpeed = 8.0f;
    [SerializeField] private float maxVelocityChange = 10.0f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private float gravityMultiplier = 2.0f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheckTransform;
    [SerializeField] private float groundDistance = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody _rigidbody;
    private Vector2 _moveInput;
    private bool _jumpRequested;
    private bool _isGrounded;
    private bool _isSprinting = false;

    private float _currentRotationX;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.useGravity = true;
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        _rigidbody.linearDamping = 5f;

        _inputActions = new InputSystem();
        _moveAction = _inputActions.Player.Move;
        _jumpAction = _inputActions.Player.Jump;
    }

    private void OnEnable()
    {
        _inputActions.Player.Enable();
        _jumpAction.performed += HandleJumpPerformed;
    }

    private void OnDisable()
    {
        _inputActions.Player.Disable();
        _jumpAction.performed -= HandleJumpPerformed;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        _moveInput = _moveAction.ReadValue<Vector2>();

        HandleRotation();
    }

    private void HandleRotation()
    {
        var pitchYaw = _inputActions.Player.Look.ReadValue<Vector2>();

        _currentRotationX -= pitchYaw.y * 1f;
        _currentRotationX = Mathf.Clamp(_currentRotationX, 0, 180);

        transform.Rotate(Vector3.up * (pitchYaw.x * 1f));

        head.localRotation = Quaternion.Euler(_currentRotationX, 0, 0);
    }

    private void FixedUpdate()
    {
        PerformGroundCheck();
        HandleMovement();
        HandleJump();
        HandleGravity();
    }

    private void PerformGroundCheck()
    {
        _isGrounded = Physics.CheckSphere(groundCheckTransform.position, groundDistance, groundLayer, QueryTriggerInteraction.Ignore);

        _rigidbody.linearDamping = _isGrounded ? 5f : 0.1f;
    }

    private void HandleMovement()
    {
        // if (!_isGrounded)
        // {
        //     return;
        // }

        float currentSpeed = _isSprinting ? sprintSpeed : moveSpeed;

        Vector3 inputDirection = transform.forward * _moveInput.y + transform.right * _moveInput.x;
        inputDirection.Normalize();

        Vector3 targetVelocity = inputDirection * currentSpeed;

        Vector3 velocity = _rigidbody.linearVelocity;
        Vector3 velocityChange = (targetVelocity - new Vector3(velocity.x, 0, velocity.z));

        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
        velocityChange.y = 0;

        _rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    private void HandleJumpPerformed(InputAction.CallbackContext context)
    {
        if (_isGrounded)
        {
            _jumpRequested = true;
        }
    }

    private void HandleJump()
    {
        if (_jumpRequested)
        {
            _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            _jumpRequested = false;
        }
    }

    private void HandleGravity()
    {
        if (!_isGrounded && _rigidbody.linearVelocity.y < 0)
        {
            _rigidbody.AddForce(Physics.gravity * (gravityMultiplier * _rigidbody.mass));
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheckTransform != null)
        {
            Gizmos.color = _isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheckTransform.position, groundDistance);
        }
    }
}