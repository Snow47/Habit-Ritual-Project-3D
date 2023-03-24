using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMotor : MonoBehaviour
{
    private enum PlayerState
    {
        Grounded,
        InAir,
        WallRide,
        Sliding
    }

    // The CharacterController simply moves the player with repects to collision, this is Kinematic Movement as it will collide with objects but not react to forces
    [SerializeField, Tooltip("This is the player's \"body\", it does the moving while respecting collision")]
    private CharacterController _controller;
    // This is the transform for the camera, for rotating the camera
    [SerializeField, Tooltip("This is the camera that should be childed to the player object, the \"Game Camera\"")]
    private Transform _head;
    [SerializeField]
    private InputActionMap _inputMap = new InputActionMap();

    [Space(10)]
    // Toggle for enabling and disabling movement
    [SerializeField, Tooltip("Whether the player can or can't move / look")]
    private bool _lockplayerController = false;

    [Header("Move Variables")]
    // How fast the player moves
    [SerializeField, Tooltip("Player max speed")]
    private float _moveSpeed = 8.0f;
    [SerializeField]
    private float _jumpHeight = 1.5f;
    [SerializeField, Tooltip("Multiplier applied to velocity when a slide is started")]
    private float _slideStrength = 1.2f;

    [Space(10)]
    [SerializeField, Tooltip("How fast the player moves")]
    private float _groundAccel = 200.0f;
    [SerializeField, Tooltip("How fast the player moves")]
    private float _airAccel = 50.0f;

    [Space(10)]
    [SerializeField]
    private float _groundFriction = 12.0f;
    [SerializeField]
    private float _airFriction = 3.0f;

    [Space(10)]
    [SerializeField]
    private float _wallClingStrength = 0.5f;
    [SerializeField, Range(0.0f, 1.0f)]
    private float _wallLaunchStrength = 0.5f;
    [SerializeField]
    private string _wallRidableTag = "Untagged";

    [Space(10)]
    // SnapDist is for a raycast so the player will smoothly slide down ramps
    [SerializeField, Tooltip("When the player stops touching the ground for a frame they will attempt to snap to any object that is this many units below theeir feet")]
    private float _snapDist = .5f;
    [SerializeField]
    private Vector3 _gravityScale = Vector3.up;

    [Header("Look Variables")]
    // Multiplier for lookDest, increases the target position
    [SerializeField]
    private float _mouseSensitivity = 3.0f;
    // How quickly the mouse moves towards lookDest
    [SerializeField]
    private float _lookSpeed = 0.5f;
    // Mouse smoothing for inbetweens
    [SerializeField]
    private float _smoothing = 2.0f;
    // Locks the rotation around the X axis (looking up and down)
    [SerializeField, Tooltip("How far up (y) and down (x) they player can look")]
    private Vector2 _vertLookExtents = new Vector2(-70.0f, 70.0f);

    [Space(10)]
    [SerializeField]
    private bool _debug = false;

    // Only needed if you need to access the head from another script and don't want to assign to there as well
    public Transform Head => _head;
    public float MoveSpeed { set => _groundAccel = value; }
    public float MouseSenMod { set => _mouseSenMod = value; }
    public bool LockPlayer 
    { 
        get => _lockplayerController; 
        set => _lockplayerController = value; 
    }

    private float SnapDist => (_controller.height / 2.0f) + _snapDist;
    private Vector3 EffectiveGravity
    {
        get
        {
            Vector3 g = Physics.gravity;
            g.Scale(_gravityScale);
            return g;
        }
    }
    private float SlideHeight => _controller.radius * 2;

    private PlayerState _currentState = PlayerState.Grounded;

    // Important for falling
    //private float _yVel = 0.0f;
    // For ground snapping
    private bool _wasGrounded = false;
    private bool _jumpWish = false;

    private float _mouseSenMod;

    // Vectors for rotation, they need to store information between frames
    private Vector2 _lookVel, _lookPos;
    // Without these the player rotation will always snap to (0, 0, 0)
    private Quaternion _rotOffset, _rotOffsetHead;

    private Vector3 _wallNormal;
    private bool _potentialWallRide;

    private float _height;
    private float _headHeight;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Set the rotation offsets
        _rotOffset = transform.rotation;
        _rotOffsetHead = _head.localRotation;
        _height = _controller.height;
        _headHeight = _head.localPosition.y;

        _mouseSenMod = PlayerPrefs.GetFloat("Player.LookSensitivity", 1.0f);

        _inputMap["Jump"].performed += AttemptJump;
        _inputMap["Slide"].started += AttemptSlide;
        _inputMap["Slide"].canceled += AttemptSlideEnd;
    }

    private void FixedUpdate()
    {
        if (_lockplayerController)
            return;

        Move();
        Look();
    }

    private void AttemptJump(InputAction.CallbackContext context)
    {
        if (_currentState == PlayerState.InAir)
            return;

        if (_currentState == PlayerState.Sliding)
            AttemptSlideEnd(context);

        _jumpWish = true;
    }
    private void AttemptSlide(InputAction.CallbackContext context)
    {
        if (_currentState != PlayerState.Grounded)
            return;

        // Begin slide
        _currentState = PlayerState.Sliding;

        // Adjust heights
        _controller.height = SlideHeight;
        Vector3 tmpHead = _head.localPosition;
        tmpHead.y = _headHeight - (_height - SlideHeight);
        _head.localPosition = tmpHead;

        // Snap down
        float snapAmount = _height - SlideHeight;
        _controller.center += Vector3.down * (snapAmount / 2.0f);

        // Kick
        Vector3 vel = _controller.velocity;

        float keepY = vel.y;
        vel.y = 0;

        vel *= _slideStrength;
        vel.y = keepY;

        _controller.Move(vel * Time.deltaTime);
    }
    private void AttemptSlideEnd(InputAction.CallbackContext context)
    {
        if (_currentState != PlayerState.Sliding)
            return;

        // End Slide
        // Adjust heights
        _controller.height = _height;
        Vector3 tmpHead = _head.localPosition;
        tmpHead.y = _headHeight;
        _head.localPosition = tmpHead;

        // Snap up
        float snapAmount = _height - SlideHeight;
        _controller.center += Vector3.up * (snapAmount / 2.0f);

        // Change flag
        if ((_controller.collisionFlags & CollisionFlags.Below) != 0)
            _currentState = PlayerState.Grounded;
        else if ((_controller.collisionFlags & CollisionFlags.Sides) != 0)
            _currentState = PlayerState.WallRide;
        else
            _currentState = PlayerState.InAir;
    }

    private void Move()
    {
        Vector3 vel = _controller.velocity;

        // JUMP
        if (_jumpWish)
        {
            _jumpWish = false;

            float jump = Mathf.Sqrt(-2.0f * EffectiveGravity.y * _jumpHeight);
            vel.y = jump;

            if(_currentState == PlayerState.WallRide)
                vel += _wallNormal * (jump * _wallLaunchStrength);

            _currentState = PlayerState.InAir;
            _wasGrounded = false;
        }

        // GRAVITY
        if (_currentState == PlayerState.WallRide)
            vel.y = EffectiveGravity.y * _wallClingStrength;
        else
            vel.y += EffectiveGravity.y * Time.deltaTime;
        
        float effectiveFriction;
        switch (_currentState)
        {
            default:
            case PlayerState.WallRide:
            case PlayerState.Grounded:
                effectiveFriction = _groundFriction;
                break;
            case PlayerState.Sliding:
            case PlayerState.InAir:
                effectiveFriction = _airFriction;
                break;
        }

        // FRICTION
        float keepY = vel.y;
        vel.y = 0.0f; // don't consider vertical movement in friction calculation
        float prevSpeed = vel.magnitude;
        if (prevSpeed != 0)
        {
            float frictionAccel = prevSpeed * effectiveFriction * Time.deltaTime;
            vel *= Mathf.Max(prevSpeed - frictionAccel, 0) / prevSpeed;
        }
        vel.y = keepY;

        // INPUT
        if(_currentState != PlayerState.Sliding)
        {
            Vector3 moveWish = _inputMap["Move"].ReadValue<Vector2>();

            // If the player is actually attempting to move
            if(moveWish != Vector3.zero)
            {
                // Get the variables on pointing the right way
                moveWish.z = moveWish.y;
                moveWish.y = 0;
                moveWish = transform.TransformDirection(moveWish).normalized;

                // Check for speed cap
                float velocityProj = Vector3.Dot(vel, moveWish);
                float effectiveAccel = _currentState != PlayerState.InAir ? _groundAccel : _airAccel;
                float accelMag = effectiveAccel * Time.deltaTime;
                // Clamp projection onto movement vector
                if (velocityProj + accelMag > _moveSpeed)
                {
                    accelMag = _moveSpeed - velocityProj;
                }

                // Apply movement
                vel += moveWish * accelMag;
            }
        }

        // MOVE
        _controller.Move(vel * Time.deltaTime);

        // SET STATE
        if(_currentState != PlayerState.Sliding)
        {
            if ((_controller.collisionFlags & CollisionFlags.Below) != 0)
                _currentState = PlayerState.Grounded;
            else if (_potentialWallRide && (_controller.collisionFlags & CollisionFlags.Sides) != 0)
                _currentState = PlayerState.WallRide;
            else
                _currentState = PlayerState.InAir;
        }

        _potentialWallRide = false;
        GroundCheck();
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag(_wallRidableTag))
        {
            _potentialWallRide = true;
            _wallNormal = hit.normal;
        }
    }

    private void GroundCheck()
    {
        // If you are grounded reset yVel and set wasGrounded for next frame
        if (_currentState == PlayerState.Grounded)
        {
            _wasGrounded = true;
            return;
        }

        // If the player is not grounded but was last frame do a Raycast and if it hits something snap to that thing
        if (_wasGrounded && _currentState != PlayerState.Sliding && Physics.Raycast(transform.position + _controller.center, Vector3.down, out RaycastHit hit, SnapDist))
        {
            // Get the distance you have to travel to be snapped to the ground
            float snapAmount = Vector3.Distance(hit.point, transform.position + _controller.center);
            // Ignore half the height as the Raycast shoots from the center of the player
            snapAmount -= _controller.height / 2.0f;
            // Convert the amount you have to move movement to a velocity scaler
            snapAmount /= Time.deltaTime;

            _controller.Move(Vector3.down * snapAmount);

            //if(_currentState != PlayerState.Sliding)
                _currentState = PlayerState.Grounded;

            return;
        }

        // Set wasGrounded for next frame
        _wasGrounded = false;
    }

    private void Look()
    {
        // get mouse input and apply scalers that are necessary for reasons I don't know but recognise
        Vector2 lookDest = _mouseSenMod * _mouseSensitivity * _inputMap["Look"].ReadValue<Vector2>();

        // smooth input
        _lookVel.Set(Mathf.Lerp(_lookVel.x, lookDest.x, 1.0f / _smoothing), Mathf.Lerp(_lookVel.y, lookDest.y, 1.0f / _smoothing));
        // Adjust lookPos beased on current look velocity
        _lookPos += _lookVel * _lookSpeed;

        // Clamp mouse look on the y axis
        _lookPos.y = Mathf.Clamp(_lookPos.y, _vertLookExtents.x, _vertLookExtents.y);
        
        // set player rotation, y rotates head, x rotates player body
        transform.rotation = Quaternion.AngleAxis(_lookPos.x, Vector3.up) * _rotOffset;
        _head.localRotation = Quaternion.AngleAxis(_lookPos.y, Vector3.left) * _rotOffsetHead;
    }

    // Simple code for when you set the rotation of the player using other code
    public void ResetRot()
    {
        _lookVel = new Vector2();
        _lookPos = new Vector2();
        _rotOffset = transform.rotation;
        _rotOffsetHead = _head.transform.localRotation;
    }
    private void OnEnable()
    {
        _inputMap.Enable();
    }

    private void OnDisable()
    {
        _inputMap.Disable();
    }

    private void OnDrawGizmos()
    {
        if (!_debug)
            return;

        Gizmos.color = Color.magenta;
        Gizmos.DrawCube(transform.position + _controller.center, new Vector3(_controller.radius * 2, _controller.height, _controller.radius * 2));
        Gizmos.DrawRay(transform.position + _controller.center, Vector3.down * SnapDist);
    }
}
