using System;
using UnityEngine;
using UnityEngine.InputSystem; 
using Configs;
using Utils;

public class PlayerController : MonoBehaviour
{ 
    public PlayerMovementConfig MovementConfig;
    public LevelConfig LevelConfig;
    
    private Vector3 _rawInputMovement;
    private Vector3 _inputMovement;
 
    private int direction = 0;
    public float timeout = 0.3f;

    private bool _isRunning = false;
    private bool _isJumping = false;
    private float _jumpStartedTime;
 
    private bool _isChangingLine = false;
    private float _changingLaneLastTime;
 
    private Vector3 _targetPosition;
    
    private int _currentLane; // todo: maybe not here
 
    private Vector3 _startingPosition;
    
    // Start is called before the first frame update
    void Awake()
    {
        if (MovementConfig == null)
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
    }
    
    public void StartRunning()
    {   
        _isRunning = true;
        _startingPosition = transform.position;
    //     StartMoving();
    //     if (character.animator)
    //     {
    //         character.animator.Play(s_RunStartHash);
    //         character.animator.SetBool(s_MovingHash, true);
    //     }
    }

    public void StopRunning()
    {
        _isRunning = false;
    }
 
    // Update is called once per frame
    void Update()
    {
        if (!_isRunning)
            return;

        Vector3 newPosition = transform.localPosition;
        newPosition.z += MovementConfig.runSpeed * Time.deltaTime;
        
        if (_isJumping)
        {
            float ratio = (Time.time - _jumpStartedTime) / MovementConfig.jumpDuration; 

            if (ratio >= 1.0f)
            {
                _isJumping = false;
                _targetPosition.y = 0;  // Ensure proper landing height
                // TODO: Set animation jumping to false
            }
            else
            {
                newPosition.y = Mathf.Sin(ratio * Mathf.PI) * MovementConfig.jumpHeight; 
            }
        }
  
        newPosition.x = Mathf.MoveTowards(newPosition.x, _targetPosition.x, 
            MovementConfig.changeLineSpeed * Time.deltaTime);
 
        transform.localPosition = newPosition;

        float distance = Vector3.Distance(_startingPosition, transform.position);
        GameManager.Instance.UpdateDistance(distance);
    }


    public void OnMovement(InputAction.CallbackContext value)
    {
        Vector2 inputMovement = value.ReadValue<Vector2>(); 
        _rawInputMovement = inputMovement;
        
        if (inputMovement.magnitude >= MovementConfig.changeLineThreshold)
        {
            direction = inputMovement.x > 0 ? 1 : -1;
            TryChangeLane(direction);
        } 
    }

    private void TryChangeLane(int inDirection)
    {
        if ((_currentLane + inDirection < 0) || (_currentLane + inDirection) >= LevelConfig.numberOfTracks)
        {
            return;
        }
            
        // TODO: ask WorldManager or WorldManagerSCOB if it's possible to change lane
        if(_changingLaneLastTime <= Time.time)
        {
            _isChangingLine = true;
            _currentLane += inDirection;
            _changingLaneLastTime = Time.time + timeout;
            _targetPosition.x += LevelConfig.trackWidth * inDirection; 
        }
    }

    public void OnJump(InputAction.CallbackContext value)
    {
        // TODO: check if running, if not dead
        
        if (value.performed && !_isJumping)
        { 
            // float correctJumpLength = jumpLength * (1.0f + trackManager.speedRatio);
            // m_JumpStart = trackManager.worldDistance;
            // float animSpeed = k_TrackSpeedToJumpAnimSpeedRatio * (trackManager.speed / correctJumpLength);
            //
            // character.animator.SetFloat(s_JumpingSpeedHash, animSpeed);
            // character.animator.SetBool(s_JumpingHash, true);
            // m_Audio.PlayOneShot(character.jumpSound);
            // m_Jumping = true;
            
            _isJumping = true;
            _jumpStartedTime = Time.time;
        }
    }

    public void OnTriggerEnter(Collider other)
    { 
        if (other.CompareTag(ProjectTags.PICKUP_TAG))
        {
            GameManager.Instance.AddCoin();
            // todo: play collect animation
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

    private float _debugTargetGizmoCubeSize = 0.2f;
    
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
