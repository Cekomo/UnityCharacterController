using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D _rbPlayer;
    
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float speed = 25f;
    
    [SerializeField] private LayerMask groundMask;
    private BoxCollider2D _boxCollider;
    
    private void Start()
    {
        _rbPlayer = GetComponent<Rigidbody2D>();
        _boxCollider = GetComponent<BoxCollider2D>();
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
        var speedDiff = Vector2.right * (speed * Time.deltaTime * movementInput);

        if (IsSpeedDiffBelowLimit(speedDiff)) 
            _rbPlayer.velocity += speedDiff;
    }
    
    private void Jump()
    {
        if (IsGrounded())
        {
            print("ping");
            _rbPlayer.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
        
        PlayerInput.CanJump = false;
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
}
