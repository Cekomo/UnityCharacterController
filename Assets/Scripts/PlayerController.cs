using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D _rbPlayer;
    
    private const float JUMP_FORCE = 7f;
    private const float SPEED = 25f;
    
    [SerializeField] private LayerMask groundMask;
    private Renderer _boxRenderer;
    
    private void Start()
    {
        _rbPlayer = GetComponent<Rigidbody2D>();
        _boxRenderer = GetComponent<Renderer>();
    }

    private void FixedUpdate()
    {
        if (PlayerInput.CanJump)
            Jump();
            
        if (PlayerInput.Direction is PlayerDirections.Left or PlayerDirections.Right)
            Move();
    }

    private void Move()
    {
        var movementInput = PlayerInput.Direction == PlayerDirections.Right ? 1 : -1;
        var previousVelocity = _rbPlayer.velocity;
        var speedAddition = Vector2.right * (SPEED * Time.deltaTime * movementInput);

        if (Mathf.Abs(previousVelocity.x) > Mathf.Abs(previousVelocity.x + speedAddition.x) || 
            Mathf.Abs(_rbPlayer.velocity.x) < 7.5f) 
            _rbPlayer.velocity += speedAddition;
    }
    
    private void Jump()
    {
        if (IsGrounded())
            _rbPlayer.AddForce(Vector2.up * JUMP_FORCE, ForceMode2D.Impulse);

        PlayerInput.CanJump = false;
    }

    private bool IsGrounded()
    {
        var raycastHit = Physics2D.BoxCast(transform.position, _boxRenderer.bounds.size, 
            0f, -Vector2.up, 0.1f, groundMask);
        return !ReferenceEquals(raycastHit.collider, null);
    }
}
