using System.Threading.Tasks; 
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using Utils;

partial class GameManager
{
    [SerializeField] private AssetReference gameplayScene;
        
    private async Task HandlePreloadingState()
    { 
        // This state plays role as a bootstrapper, it need to make sure it have all dependencies ready before loading into the gameplay
        while (!(PoolManager.Instance && PoolManager.Instance.IsInitialized))
        {
            await Task.Yield();
        } 
        var sceneLoadHandle = Addressables.LoadSceneAsync(gameplayScene, LoadSceneMode.Single);
        await sceneLoadHandle.Task;
        if (sceneLoadHandle.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log("GameplayScene successfully loaded");
        }
        else
        {
            Debug.LogError("Failed to load gameplay scene");
        }

        UpdateGameState(GameState.MainMenu);
    }
}