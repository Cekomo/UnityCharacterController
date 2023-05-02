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
            var speedDiff = Vector2.right * (speed * Time.deltaTime * horizontalVector);
            
            if (horizontalVector.x == 0f || !IsSpeedDiffBelowLimit(speedDiff)) return;
           
            _rbPlayer.velocity += speedDiff;
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
        
        private bool IsSpeedDiffBelowLimit(Vector2 speedDifference)
        {
            var previousVelocity = _rbPlayer.velocity;

            return Mathf.Abs(previousVelocity.x) > Mathf.Abs(previousVelocity.x + speedDifference.x) || 
                   Mathf.Abs(_rbPlayer.velocity.x) < 7.5f;
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