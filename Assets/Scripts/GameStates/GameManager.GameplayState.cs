using System;
using System.Threading.Tasks;

public partial class GameManager
{
    public float DistanceRun { get; private set; }
    public int CoinsCollected { get; private set; }

    public event Action<float> OnDistanceUpdated;
    public event Action<int> OnCoinsUpdated;
        
    public void UpdateDistance(float newDistance)
    {
        DistanceRun = newDistance;
        OnDistanceUpdated?.Invoke(DistanceRun);
    }

    public void AddCoin()
    {
        CoinsCollected++;
        OnCoinsUpdated?.Invoke(CoinsCollected);
    }

    private async Task EnterGamePlayState(GameState previousState)
    {
        while (!LevelGenerator.Instance) 
            await Task.Yield();
     
        CoinsCollected = 0;
        DistanceRun = 0;
        
        if (previousState == GameState.GameOver)
        {
            await LevelGenerator.Instance.UnloadAllSegmentsAsync(); 
        }

        await LevelGenerator.Instance.GenerateTrackFromStartAsync();
    }
}