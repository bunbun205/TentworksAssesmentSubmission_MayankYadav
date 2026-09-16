using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementComponent : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [Header("Input")] public InputActionReference move;

    [Header("Physics")] private Rigidbody _playerRB;

    [Header("Movement")] 
    [SerializeField] private float moveSpeed = 5f;

    private Vector2 _moveInput;

    private void OnEnable()
    {
        move.action.Enable();
        
        move.action.performed += OnMove;
        move.action.canceled += OnMove;
    }

    private void OnDisable()
    {
        move.action.performed -= OnMove;
        move.action.canceled -= OnMove;
        
        move.action.Disable();
    }

    private void Awake()
    {
        _playerRB = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        _moveInput = ctx.ReadValue<Vector2>();
    }

    private void Move()
    {
        Vector3 direction = new Vector3(_moveInput.x, 0f, _moveInput.y);
        Vector3 targetVelocity = direction * moveSpeed;

        Vector3 velocity = _playerRB.linearVelocity;
        velocity.x = targetVelocity.x;
        velocity.z = targetVelocity.z;

        _playerRB.linearVelocity = velocity;
    }
}
