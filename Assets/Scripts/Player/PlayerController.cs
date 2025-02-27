using UnityEngine;
using UnityEngine.InputSystem; 
using Configs; 

public class PlayerController : MonoBehaviour
{ 
    [SerializeField] private PlayerMovementConfig movementConfig;
    [SerializeField] private LevelConfig levelConfig;
    
    private Vector3 _rawInputMovement;
    private Vector3 _inputMovement;
 
    private int _horizontalDirection = 0;
  
    private bool _isRunning = false;
    private bool _isJumping = false;
    private float _jumpStartedTime;
 
    private bool _isChangingLine = false;
    private float _currentChangeLineTimeout; 
    private int _currentLane; 
 
    private Vector3 _startingPosition;
    private Vector3 _targetPosition;
    
    // ANIMATIONS
    private readonly int _animationGameOverHash = Animator.StringToHash ("GameOver");
    private readonly int _animationJumpingHash = Animator.StringToHash("Jumping");
    private readonly int _animationRunning = Animator.StringToHash("Running"); 
        
    [SerializeField] private Animator animator;

    private InputAction _swipeAction;
    private Vector2 _swipeDelta;
 
    // Start is called before the first frame update
    void Awake()
    {
        if (movementConfig == null)
        {
            Debug.LogError("Movement Config is missing. You forgot to assign MovementConfig to PlayerMovementConfig", gameObject);
        }

        GameManager.OnGameStateChanged += HandleGameStateChanged;
        HandleGameStateChanged(GameManager.State);
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= HandleGameStateChanged;
    }
  
    public void PlaceAtPositionAndRun(Vector3 position, int line)
    {
        _startingPosition = transform.position;
        _targetPosition = Vector3.zero;
        transform.position = position;
        _currentLane = line;
        
        StartRunning();
    }

    private void HandleGameStateChanged(GameState state)
    { 
        if (state == GameState.GameOver)
        {
            StopRunning();
        } 
        else if (state == GameState.GamePlay)
        {
            animator.SetBool(_animationGameOverHash, false); 
        }
    }
    
    private void StartRunning()
    {   
        _isRunning = true;
        _startingPosition = transform.position;
        animator.SetBool(_animationRunning, true); 
    }

    private void StopRunning()
    {
        _isRunning = false;
        animator.SetBool(_animationRunning, true); 
        animator.SetBool(_animationGameOverHash, true); 
 
    }
 
    // Update is called once per frame
    private void Update()
    {
        if (!_isRunning)
            return;

        Vector3 newPosition = transform.localPosition;
        newPosition.z += movementConfig.runSpeed * Time.deltaTime;
        
        if (_isJumping)
        {
            float ratio = (Time.time - _jumpStartedTime) / movementConfig.jumpDuration; 

            if (ratio >= 1.0f)
            {
                _isJumping = false;
                // Ensure proper landing height 
                _targetPosition.y = _startingPosition.y; 
            }
            else
            {
                newPosition.y = Mathf.Sin(ratio * Mathf.PI) * movementConfig.jumpHeight;
                newPosition.z += movementConfig.jumpForwardMultiplier * Time.deltaTime;
            }
        }
  
        newPosition.x = Mathf.MoveTowards(newPosition.x, _targetPosition.x, 
            movementConfig.changeLineSpeed * Time.deltaTime);
 
        transform.localPosition = newPosition;

        float runDistance = Vector3.Distance(_startingPosition, transform.position);
        GameManager.Instance.UpdateDistance(runDistance);
    }


#region "Input handling"

    public void OnMovement(InputAction.CallbackContext value)
    {
        if (value.phase != InputActionPhase.Performed)
        {
            return;
        }
        
        var inputMovement = value.ReadValue<Vector2>(); 
 
        if (Mathf.Abs(inputMovement.x) > movementConfig.inputMovementThreshold)
        {
            _horizontalDirection = inputMovement.x > 0 ? 1 : -1;
            TryChangeLane(_horizontalDirection);
        } 
    }

    public void OnSwipe(InputAction.CallbackContext value)
    { 
        _swipeDelta = value.ReadValue<Vector2>();
        if (_swipeDelta.magnitude < movementConfig.inputMovementThreshold)
        {
            return;
        }
        
        // Process Swipe
        var x = Mathf.Abs(_swipeDelta.x);
        var y = Mathf.Abs(_swipeDelta.y);

        // only handle horizontal or vertical input at once
        if (x > y)
        {
            _horizontalDirection = _swipeDelta.x > 0 ? 1 : -1;
            TryChangeLane(_horizontalDirection);
        }
        else  
        {
            if (_swipeDelta.y > 0)
            {
                TryJump();
            }
        }
    }

    public void OnJump(InputAction.CallbackContext value)
    { 
        if (value.performed)
        {
            TryJump();
        }
    }
 
#endregion "Input handling"

    private void TryJump()
    {
        if(!_isJumping && _isRunning)
        {
            animator.SetTrigger(_animationJumpingHash);
            _isJumping = true;
            _jumpStartedTime = Time.time;
        }
    }
    
    private void TryChangeLane(int inDirection)
    {
        if ((_currentLane + inDirection < 0) || (_currentLane + inDirection) >= levelConfig.numberOfTracks)
        {
            return;
        }
            
        if(_currentChangeLineTimeout <= Time.time)
        {
            _isChangingLine = true;
            _currentLane += inDirection;
            _currentChangeLineTimeout = Time.time + movementConfig.changeLineTimeout;
            _targetPosition.x += levelConfig.trackWidth * inDirection; 
        }
    }
 
    public void OnTriggerEnter(Collider other)
    { 
        if (other.CompareTag(ProjectTags.PICKUP_TAG))
        {
            GameManager.Instance.AddCoin();
            // TODO: play collect animation
            other.gameObject.SetActive(false); 
        } 
        else if (other.CompareTag(ProjectTags.OBSTACLE_TAG))
        {
            GameManager.Instance.UpdateGameState(GameState.GameOver);
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
    }

    #region "Debug"

    private readonly float _debugTargetGizmoCubeSize = 0.2f;
    
    private void OnDrawGizmos()
    {
        if (_isChangingLine)
        { 
            Gizmos.color = Color.green; 
            Gizmos.DrawLine(transform.position, _targetPosition); 
            Gizmos.DrawCube(_targetPosition, Vector3.one * _debugTargetGizmoCubeSize);
        } 
    }

    #endregion "Debug"
}
