using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class MovementController : MonoBehaviour
    {
        private PlayerInputActions _playerInputActions;
        private InputAction movement;

        private Rigidbody2D _rbPlayer => GetComponent<Rigidbody2D>();
        private BoxCollider2D _boxCollider => GetComponent<BoxCollider2D>();
        [SerializeField] private LayerMask groundMask;

        [SerializeField] private float jumpForce;
        [SerializeField] private float speed;
        

        private void Awake()
        {
            _playerInputActions = new PlayerInputActions();
        }

        private void FixedUpdate()
        {
            Move();
        }

        private void Move()
        {
            var horizontalVector = movement.ReadValue<Vector2>();
            if (horizontalVector.x == 0f) return;
            
            _rbPlayer.velocity += Vector2.right * horizontalVector * (speed * Time.deltaTime);
        }
        
        private void Jump(InputAction.CallbackContext obj)
        {
            if (IsGrounded())
                _rbPlayer.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
        
        private bool IsGrounded()
        {
            var raycastHit = Physics2D.BoxCast(transform.position, _boxCollider.bounds.size, 
                0f, -Vector2.up, 0.1f, groundMask);
        
            return !ReferenceEquals(raycastHit.collider, null);
        }
        
        private void OnEnable()
        {
            movement = _playerInputActions.Player.Move;
            movement.Enable();

            _playerInputActions.Player.Jump.performed += Jump;
            _playerInputActions.Player.Jump.Enable();
        }

        private void OnDisable()
        {
            _playerInputActions.Disable();
        }
    }
}

/*
 * public class MovementController : MonoBehaviour
    {
        private PlayerInputDefault _playerControls;

        private Rigidbody2D _rbPlayer => GetComponent<Rigidbody2D>();

        private void Awake()
        {
            _playerControls = new PlayerInputDefault();
        }

        private void Start()
        {
            _playerControls.Player.Move.performed += Move;
        }

        private void Move(InputAction.CallbackContext context)
        {
            var horizontalVector = context.ReadValue<Vector2>();
            _rbPlayer.velocity += Vector2.right * horizontalVector * 10f;
        }

        private void OnEnable()
        {
            _playerControls.Enable();
        }

        private void OnDisable()
        {
            _playerControls.Disable();
        }
    }
 */

/*
 * private Rigidbody2D _rbPlayer => GetComponent<Rigidbody2D>();
        
        [SerializeField] private float jumpForce = 7f;
        [SerializeField] private float speed = 25f;
        
        [SerializeField] private LayerMask groundMask;
 */