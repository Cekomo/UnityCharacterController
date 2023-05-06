using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class MovementCurveController : MonoBehaviour
    {
        private MovementCurve movementCurves;

        public static HorizontalDirection HorizontalDirection;
        
        private PlayerInputActions _playerInputActions;
        private InputAction movement;
        private HorizontalDirection oldPlayerDirection;
        
        private Rigidbody2D _rbPlayer => GetComponent<Rigidbody2D>();
        private BoxCollider2D _boxCollider => GetComponent<BoxCollider2D>();
        [SerializeField] private LayerMask groundMask;
        
        [SerializeField] private float jumpForce;
        [SerializeField] private float speed;
        [SerializeField] private float speedLimit;
        
        private float _curveTimer;
        private const float CURVE_PERIOD = 1f;
        
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
                movementCurves.movementCurve[0].Evaluate(CurveElapsedTime(ref _curveTimer, movementVector.x));
        
            _rbPlayer.velocity = speedDirection * currentCurve;
            print(_rbPlayer.velocity);
        }
        
        private float CurveElapsedTime(ref float curveTimer, float horizontalVector)
        {
            // if curve-ending slowing process changes, keyCount should be changed
            var keyCount = movementCurves.movementCurve[0].keys.Length;
            var lastKeyTime = movementCurves.movementCurve[0].keys[keyCount-2].time;
            
            if ((curveTimer < lastKeyTime && horizontalVector != 0f) ||
                (curveTimer >= lastKeyTime && horizontalVector == 0f))
                curveTimer += Time.deltaTime;
            
            if (curveTimer >= CURVE_PERIOD || 
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
        
        private void OnEnable()
        {
            movement = _playerInputActions.Player.Move;
            movement.Enable();
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