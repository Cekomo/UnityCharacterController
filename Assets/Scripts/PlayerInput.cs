using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private InputActionReference movement, jump;
    private static Vector2 movementInput;
    private float jumpInput;

    public static bool CanJump;

    public static PlayerDirections Direction;
    
    private void Update()
    {
        movementInput = movement.action.ReadValue<Vector2>();
        jumpInput = jump.action.ReadValue<float>();

        if (jumpInput >= 0.5f)
            StartCoroutine(PermitJumping());
        
        DetectDirection();
    }
    
    private static void DetectDirection()
    {
        switch (movementInput)
        {
            case { y: > 0.5f }:
                Direction = PlayerDirections.Up;
                break;
            case { y: < -0.5f }:
                Direction = PlayerDirections.Down;
                break;
            case { x: > 0.5f }:
                Direction = PlayerDirections.Right;
                break;
            case { x: < -0.5f }:
                Direction = PlayerDirections.Left;
                break;
            default:
                Direction = PlayerDirections.Idle;
                break;
        }
    }

    private static IEnumerator PermitJumping()
    {
        CanJump = true;
        yield return new WaitForSeconds(0.1f);
        CanJump = false;
    }
}



public enum PlayerDirections
{
    Idle,
    Up,
    Down,
    Left, 
    Right,
}
