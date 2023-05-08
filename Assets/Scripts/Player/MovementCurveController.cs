using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class MovementCurveController : MonoBehaviour
    {
        private MovementCurve movementCurves;

        private PlayerInputActions _playerInputActions;
        private InputAction movement;
        private HorizontalDirection oldPlayerDirection;
        
        private Rigidbody2D _rbPlayer => GetComponent<Rigidbody2D>();
        private BoxCollider2D _boxCollider => GetComponent<BoxCollider2D>();
        [SerializeField] private LayerMask groundMask;
        
        [SerializeField] private float jumpForce;
        [SerializeField] private float speed;

        private float _movementCurveTimer;
        private float _lastKeyTime;
        private const float MOVE_CURVE_PERIOD = 1f;
        private float _directionFloat;
        
        private float _jumpingCurveTimer;
        private float _jumpCurvePeriod;
        
        private void Awake()
        {
            _playerInputActions = new PlayerInputActions();
            movementCurves = GetComponent<MovementCurve>();
        }
        
        private void FixedUpdate()
        {
            Move();
        }

        private void Move()
        {
            var movementVector = movement.ReadValue<Vector2>();
            var speedDirection = Vector2.right * movementVector * (speed * Time.deltaTime);
            var currentCurve = 
                movementCurves.movementCurve[0].Evaluate(CurveElapsedTime(ref _movementCurveTimer, movementVector.x));

            if (_movementCurveTimer < _lastKeyTime && movementVector.x == 0f)
                _movementCurveTimer = _lastKeyTime;
            
            if (DetectPlayerDirection() == HorizontalDirection.Left)
                _directionFloat = -1;
            else if (DetectPlayerDirection() == HorizontalDirection.Right)
                _directionFloat = 1;
            
            if (_movementCurveTimer >= _lastKeyTime && _movementCurveTimer < MOVE_CURVE_PERIOD && movementVector.x == 0f)
                speedDirection = Vector2.right * (speed * Time.deltaTime * _directionFloat);
            
            _rbPlayer.velocity = new Vector2(speedDirection.x * currentCurve, _rbPlayer.velocity.y);
        }

        private void Jump(InputAction.CallbackContext obj)
        {
            if (!IsGrounded()) return;
      
            var currentCurve = movementCurves.movementCurve[5];
            
            StartCoroutine(JumpUsingCurve(currentCurve));

            // _rbPlayer.velocity += Vector2.up * jumpForce;
            // _rbPlayer.velocity += Vector2.up * jumpForce * Time.deltaTime * 500;
        }
        
        private bool IsGrounded()
        {
            var raycastHit = Physics2D.BoxCast(transform.position, _boxCollider.bounds.size, 
                0f, -Vector2.up, 0.1f, groundMask);
            
            return !ReferenceEquals(raycastHit.collider, null);
        }
        
        private float CurveElapsedTime(ref float curveTimer, float horizontalVector)
        {
            // if curve-ending slowing process changes, keyCount should be changed
            var keyCount = movementCurves.movementCurve[0].keys.Length;
            _lastKeyTime = movementCurves.movementCurve[0].keys[keyCount-2].time;
            
            if ((curveTimer < _lastKeyTime && horizontalVector != 0f) ||
                (curveTimer >= _lastKeyTime && horizontalVector == 0f))
                curveTimer += Time.deltaTime;
            
            if ((curveTimer >= MOVE_CURVE_PERIOD && horizontalVector != 0f) || 
                (DetectPlayerDirection() != oldPlayerDirection && horizontalVector != 0f)) 
                curveTimer = 0f;
            
            oldPlayerDirection = DetectPlayerDirection();
            
            return curveTimer;
        }

        private HorizontalDirection DetectPlayerDirection()
        {
            var horizontalVector = movement.ReadValue<Vector2>();

            switch (horizontalVector.x)
            {
                case >= 0.1f:
                    return HorizontalDirection.Right;
                case <= -0.1f:
                    return HorizontalDirection.Left;
                default:
                    return HorizontalDirection.Idle;
            }
        }

        private IEnumerator JumpUsingCurve(AnimationCurve currentCurve)
        {
            _jumpingCurveTimer = 0f;
            _jumpCurvePeriod = currentCurve.keys[^1].time;
            while (_jumpingCurveTimer < _jumpCurvePeriod)
            {
                _rbPlayer.velocity += Vector2.up * (currentCurve.Evaluate(_jumpingCurveTimer) * jumpForce) / 100;
                _jumpingCurveTimer += Time.deltaTime;
                yield return null;
            }
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

    public enum HorizontalDirection
    {
        Idle, 
        Right,
        Left
    }
}