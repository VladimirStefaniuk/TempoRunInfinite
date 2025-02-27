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
    
    void Awake()
    {
        DontDestroyOnLoad(this);
        Instance = this;
    }

    void Start()
    {
        UpdateGameState(GameState.PreLoading);
    }
    
    public void UpdateGameState(GameState newState)
    {
        _stateQueue.Enqueue(newState);
        ProcessStateQueue();
    }

    private bool isProcessingState;
    
    private async void ProcessStateQueue()
    { 
        if (isProcessingState) return; // Prevent reentrancy
        isProcessingState = true;
        while (_stateQueue.Count > 0)
        {
            GameState nextState = _stateQueue.Dequeue();
            await UpdateGameStateAsync(nextState);
        } 
        isProcessingState = false;
    }
    
    public async Task UpdateGameStateAsync(GameState newState)
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