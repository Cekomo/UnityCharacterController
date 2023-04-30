using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D _rbPlayer;
    
    [SerializeField] private InputActionReference movement, jump;
    private Vector2 movementInput;
    private float jumpInput;
    
    private const float JUMP_FORCE = 7f;
    private const float SPEED = 25f;
    
    [SerializeField] private LayerMask groundMask;
    private Renderer _boxRenderer;
    
    private void Start()
    {
        _rbPlayer = GetComponent<Rigidbody2D>();
        _boxRenderer = GetComponent<Renderer>();
    }
    
    private void Update()
    {
        movementInput = movement.action.ReadValue<Vector2>();
        jumpInput = jump.action.ReadValue<float>();
    }

    private void FixedUpdate()
    {
        if (jumpInput > 0.5f)
            Jump();
            
        if (Mathf.Abs(movementInput.x) < 0.1f) return;

        Move();
    }

    private void Move()
    {
        _rbPlayer.velocity += Vector2.right * (SPEED * Time.deltaTime * movementInput.x);
    }
    
    private void Jump()
    {
        if (IsGrounded())
            _rbPlayer.AddForce(Vector2.up * JUMP_FORCE, ForceMode2D.Impulse);
    }

    private bool IsGrounded()
    {
        var raycastHit = Physics2D.BoxCast(transform.position, _boxRenderer.bounds.size, 
            0f, -Vector2.up, 0.1f, groundMask);
        return !ReferenceEquals(raycastHit.collider, null);
    }
}
