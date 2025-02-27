using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

// TODO: upgrade states to classes, with OnEnter, OnExit, interfaces. Current approach is good for game at this scale of prototyping. 
public partial class GameManager : MonoBehaviour
{ 
    public static GameManager Instance { get; private set; }

    public static GameState State { get; private set; }
    
    public static Action<GameState> OnGameStateChanged;
    
    private readonly Queue<GameState> _stateQueue = new Queue<GameState>();
    
    private bool _isProcessingState;
    
    private void Awake()
    {
        DontDestroyOnLoad(this);
        Instance = this;
    }

    private void Start()
    {
        UpdateGameState(GameState.PreLoading);
    }
    
    public async void UpdateGameState(GameState newState)
    {
        try
        {
            _stateQueue.Enqueue(newState);
            await ProcessStateQueue();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
 
    private async Task ProcessStateQueue()
    { 
        if (_isProcessingState) return; // Prevent reentrancy
        _isProcessingState = true;
        while (_stateQueue.Count > 0)
        {
            GameState nextState = _stateQueue.Dequeue();
            await UpdateGameStateAsync(nextState);
        } 
        _isProcessingState = false;
    }

    private async Task UpdateGameStateAsync(GameState newState)
    { 
        switch (newState)
        {
            case GameState.PreLoading:
                await HandlePreloadingState();
                break;
            case GameState.MainMenu:
                break;
            case GameState.GamePlay:
                await EnterGamePlayState(previousState: State); 
                break;
            case GameState.GameOver:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
        
        State = newState;
        OnGameStateChanged?.Invoke(State);
    }
}