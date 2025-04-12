using System;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
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

    private CharacterController _characterController;
    private InputManager _inputManager = null!;
    private Vector2 _moveInput;
    private bool _jumpRequested;

    private float _currentRotationX;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();

    }

    private void OnEnable()
    {
    }

    private void OnDisable()
    {
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _inputManager = InputManager.Instance;
        _inputActions = _inputManager.PlayerControls;
        _moveAction = _inputActions.Player.Move;
        _jumpAction = _inputActions.Player.Jump;
    }

    private void Update()
    {
        _moveInput = _moveAction.ReadValue<Vector2>();

        HandleRotation();
    }

    private void HandleRotation()
    {
        var pitchYaw = _inputActions.Player.Look.ReadValue<Vector2>();

        _currentRotationX -= pitchYaw.y * 1f;
        _currentRotationX = Mathf.Clamp(_currentRotationX, -90f, 90f);

        transform.Rotate(Vector3.up * (pitchYaw.x * 1f));

        head.localRotation = Quaternion.Euler(_currentRotationX, 0, 0);
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleJump();
    }

    private void HandleMovement()
    {
        // if (!_isGrounded)
        // {
        //     return;
        // }


        Vector3 inputDirection = transform.forward * _moveInput.y + transform.right * _moveInput.x;
        inputDirection.Normalize();

        Vector3 targetVelocity = inputDirection * moveSpeed;

        Vector3 velocity = _characterController.velocity;
        Vector3 velocityChange = (targetVelocity - new Vector3(velocity.x, 0, velocity.z));

        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
        velocityChange.y = 0;

        if (_characterController.isGrounded)
        {
            _characterController.Move(velocityChange * Time.fixedDeltaTime);
        }
        else
        {
            _characterController.Move(Physics.gravity * (gravityMultiplier * Time
                .fixedDeltaTime));
        }
    }

    private void HandleJumpPerformed(InputAction.CallbackContext context)
    {
        if (_characterController.isGrounded)
        {
            _jumpRequested = true;
        }
    }

    private void HandleJump()
    {
        if (_jumpRequested)
        {
            _characterController.Move(Vector3.up * (jumpForce * Time.fixedDeltaTime));
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheckTransform)
        {
            Gizmos.color = _characterController.isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheckTransform.position, groundDistance);
        }
    }
}