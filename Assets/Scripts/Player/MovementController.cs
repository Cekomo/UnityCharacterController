using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class MovementController : MonoBehaviour
    {
        public MovementCurve movementCurves;
        
        private PlayerInputActions _playerInputActions;
        private InputAction movement;

        private Rigidbody2D _rbPlayer => GetComponent<Rigidbody2D>();
        private BoxCollider2D _boxCollider => GetComponent<BoxCollider2D>();
        [SerializeField] private LayerMask groundMask;

        [SerializeField] private float jumpForce;
        [SerializeField] private float speed;

        public bool isCurveIncluded;
        private float timer;

        private void Awake()
        {
            _playerInputActions = new PlayerInputActions();
        }

        private void FixedUpdate()
        {
            Move();
        }

        // create a timer function that starts from zero with movement press and reaches the latest point 
        // where the curve at max, timer should not go further until the key is released, 
        // timer will start from zero, goes until x (btw 0 and 1) as far as keypress and concludes at 1 when released

        private void Move()
        {
            var horizontalVector = movement.ReadValue<Vector2>();
            if (timer >= 1f) timer = 0f;
            var speedDiff = Vector2.right * (speed * Time.deltaTime * horizontalVector);
            var currentCurve = movementCurves.movementCurve[0].Evaluate(CurveElapsedTime(ref timer, horizontalVector));

            if (!IsSpeedDiffBelowLimit(speedDiff)) return;

            var currentVelocity = _rbPlayer.velocity;
            if (isCurveIncluded)
            {
                _rbPlayer.velocity = currentVelocity + speedDiff * currentCurve;
            }
            else
            {
                _rbPlayer.velocity = currentVelocity + speedDiff;
            }
                
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

        private float CurveElapsedTime(ref float curveTimer, Vector2 horizontalVector)
        {
            var lastKeyTime = movementCurves.movementCurve[0].keys[2].time;
            if (curveTimer < lastKeyTime || horizontalVector.x == 0f)
                curveTimer += Time.deltaTime;
       
            return curveTimer;
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