using UnityEngine.InputSystem;
using UnityEngine;

namespace Player
{
    public class InputConverter : MonoBehaviour
    {
        public static PlayerDirections Directions;
        private PlayerDirections _previousDirection = PlayerDirections.Idle;
        
        private PlayerInputActions _playerInputActions;
        private InputAction _movement;
        
        private void Awake()
        {
            _playerInputActions = new PlayerInputActions();
        }
        
        private void Update()
        {
            var horizontalVector = _movement.ReadValue<Vector2>();
            print(horizontalVector);
            
            switch (horizontalVector.x)
            {
                case >= 0.1f:
                    Directions = PlayerDirections.Right;
                    break;
                case <= -0.1f:
                    Directions = PlayerDirections.Left;
                    break;
                default:
                    Directions = PlayerDirections.Idle;
                    break;
            }
            print(Directions);
            _previousDirection = Directions;
        }
        
        private void OnEnable()
        {
            _movement = _playerInputActions.Player.Move;
            _movement.Enable();
        }

        private void OnDisable()
        {
            _playerInputActions.Disable();
        }
    }
    
    public enum PlayerDirections
    {
        Idle,
        Up,
        Down,
        Left,
        Right
    }
}