using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Configs;
using Utils;
using Random = UnityEngine.Random;

public struct LoadedSegment
{ 
    public LevelSegment Segment;  
    public List<GameObject> Obstacles;
    public List<GameObject> Pickups;
}

public class LevelGenerator : MonoBehaviour
{
    public static LevelGenerator Instance { get; private set; }
    
    [SerializeField] private LevelConfig levelConfig;   
    [SerializeField] private PlayerController playerController; 
  
    private PoolManager PoolManager => PoolManager.Instance;

    private List<LoadedSegment> _segments; 
    private bool _isFirstSegmentIgnored;   
    private Task _updateTrackTask = Task.CompletedTask; 
    private GameState _currentState;

    private void Awake()
    {
        Instance = this;
        _segments = new List<LoadedSegment>(levelConfig.maxLoadedSegments);
        GameManager.OnGameStateChanged += HandleOnGameStateChanged;
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= HandleOnGameStateChanged;
    }

    private void HandleOnGameStateChanged(GameState state)
    {    
        _currentState = state;
    }
 
    public async Task HandleOnExitTriggerEntered()
    {
        if (_currentState != GameState.GamePlay)
            return;
        
        if (!_isFirstSegmentIgnored)
        {
            _isFirstSegmentIgnored = true;
            return;
        }
        
        UnloadTopSegment();  
        _updateTrackTask = AddNextRandomSegmentAsync();
        await _updateTrackTask; 
    }
 
    public async Task GenerateTrackFromStartAsync()
    {
        await _updateTrackTask;
        
        _isFirstSegmentIgnored = false; 
        _segments.Clear();
        
        var entrySegmentGo = PoolManager.Get(levelConfig.alwaysFirstSegment);
        if (entrySegmentGo == null)
        {
            Debug.LogError("Failed to load alwaysFirstSegment!");
            return;
        }

        await AddNewSegmentAsync(entrySegmentGo);
        
        var loadSegmentTasks = new List<Task>();
        for (int i = 0; i < levelConfig.maxLoadedSegments; ++i)
        {
            loadSegmentTasks.Add(AddNextRandomSegmentAsync());
        } 
        
        await Task.WhenAll(loadSegmentTasks);

        PlacePlayerAtStart();
    }

    private async Task AddNextRandomSegmentAsync()
    {
        var randomSegment = levelConfig.segments[Random.Range(0, levelConfig.segments.Length)]; 
        if (!randomSegment.RuntimeKeyIsValid())
        {
            Debug.LogError($"Invalid random segment: {randomSegment}");
            return;
        } 
        var segment = PoolManager.Get(randomSegment);
        await AddNewSegmentAsync(segment);
    }
    
    private async Task AddNewSegmentAsync(GameObject segmentGo)
    {   
        var segment = segmentGo.GetComponent<LevelSegment>(); 
        var obstacles = new List<GameObject>(); 
        var pickups = new List<GameObject>();
        
        if (!segment.isManuallyPrebuild)
        {
            var rowsInSegment = Mathf.FloorToInt(Math.Abs(segment.Exit.position.z - segment.Entrance.position.z) /
                                                levelConfig.rowLength);
            float firstRowZ = segment.Entrance.position.z + levelConfig.rowLength / 2;
            
            // GENERATE COINS
            int trackForCoins = -1;
            int rowForCoins = -1;
            int amountOfCoins = 0;

            bool isCoinGenerated = Random.Range(0.0f, 1.0f) <= segment.chanceOfCoins;
            if (isCoinGenerated)
            {
                amountOfCoins = Random.Range(levelConfig.minSequenceOfCoins,
                    Math.Min(levelConfig.maxSequenceOfCoins, rowsInSegment));
                trackForCoins = Random.Range(0, levelConfig.numberOfTracks);
                rowForCoins = Random.Range(0, rowsInSegment - amountOfCoins);
                
                for (int row = rowForCoins; row < rowsInSegment; ++row)
                {
                    Vector3 coinPosition = segment.Entrance.position;
                    coinPosition.z = firstRowZ + levelConfig.rowLength * row;
                    coinPosition.x += (levelConfig.startingTrackIndex - trackForCoins) * levelConfig.trackWidth;

                    var coin = PoolManager.Get(levelConfig.coinPrefab);
                    if (coin.transform.parent.name.Contains("Segment"))
                    {
                        Debug.LogError("Something went wrong!");
                    }
                    coin.transform.SetParent(segment.transform, false);
                    coin.transform.localPosition = coinPosition;
                    coin.SetActive(true);

                    pickups.Add(coin);
                }
            }

            await Task.Yield();
            
            // GENERATE OBSTACLES 
 
            for (int track = 0; track < levelConfig.numberOfTracks; ++track)
            {
                int obstaclesGenerated = 0;
                for (int row = 0; row < rowsInSegment; ++row)
                {
                    if (obstaclesGenerated >= levelConfig.numberOfTracks - 1)
                        continue;

                    bool isOccupiedByCoin = trackForCoins == track && row >= rowForCoins &&
                                            row < (rowForCoins + amountOfCoins);
                    if (isOccupiedByCoin)
                    {
                        continue;
                    }

                    bool isObstacleGenerated = Random.Range(0.0f, 1.0f) <= segment.chanceOfObstacle;
                    if (!isObstacleGenerated)
                        continue;

                    obstaclesGenerated++;

                    var randomObstacle = levelConfig.obstacles[Random.Range(0, levelConfig.obstacles.Length)];
                    Vector3 obstaclePos = segment.Entrance.position;
                    obstaclePos.z = firstRowZ + levelConfig.rowLength * row;
                    obstaclePos.x += (levelConfig.startingTrackIndex - track) * levelConfig.trackWidth;

                    var obstacle = PoolManager.Get(randomObstacle);
                    obstacles.Add(obstacle);

                    obstacle.transform.SetParent(segment.transform, false);
                    obstacle.transform.Translate(obstaclePos, Space.Self);
                    obstacle.SetActive(true);
                }
            }
        }
        
        RegisterTrackSegment(segment, pickups, obstacles);
        
        segmentGo.transform.SetParent(transform);
        segmentGo.SetActive(true);
    }

    private void RegisterTrackSegment(LevelSegment segment, List<GameObject> obstacles, List<GameObject> pickups)
    { 
        segment.Init(this);
        if (_segments.Any())
        {
            Vector3 offset = segment.Entrance.position - segment.transform.position;
            segment.transform.position = _segments[^1].Segment.Exit.position - offset;
        }  
        else // First segment must be explicitly placed at zero
        {
            segment.transform.position = Vector3.zero;
        }
        
        _segments.Add(new LoadedSegment() { 
            Segment = segment,
            Pickups = pickups,
            Obstacles = obstacles }); 
    }

    private void PlacePlayerAtStart()
    {
        var firstSegment = _segments.First();
        playerController.PlaceAtPositionAndRun(firstSegment.Segment.Entrance.position, levelConfig.startingTrackIndex);
    }

    public async Task UnloadAllSegmentsAsync()
    {
        while (_segments.Any())
        {
            UnloadTopSegment();
            await Task.Yield();
        }
    }
    
    private void UnloadTopSegment()
    {
        if (!_segments.Any())
        {
            Debug.LogError("No segments to unload");
            return;
        }

        // recycle pickups & obstacles
        var unloadingSegment = _segments[0];
        foreach (var t in unloadingSegment.Pickups)
        { 
            PoolManager.Recycle(t);
        }
        foreach (var t in unloadingSegment.Obstacles)
        {
            PoolManager.Recycle(t);
        }
  
        PoolManager.Recycle(unloadingSegment.Segment.gameObject);
        _segments.RemoveAt(0);
    }
 
    // TODO: Missing functionality.
    // 1. Since we always move player in Z direction after a long time,
    // z value will become so large that float will start to loose precision
    // the common technique is to "move" track & player at 0, 0, 0, to start Z calculations from 0 again
    // 2. Implement algo to make sure level always have a path. 
}

