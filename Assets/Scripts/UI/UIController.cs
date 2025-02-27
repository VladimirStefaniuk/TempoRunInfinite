using System.Threading.Tasks;
using UnityEngine;

namespace UI
{
    public class UIController : MonoBehaviour
    {
        public static UIController Instance;
        
        [SerializeField] private GameObject mainMenuStateGo;
        [SerializeField] private GameObject gameOverStateGo;
        [SerializeField] private GameObject pauseStateGo;
        [SerializeField] private GameObject gameplayStateGo;

        [SerializeField] private GameObject loadingScreen; 
        
        private void Awake()
        {
            Instance = this;
            GameManager.OnGameStateChanged += HandleGameStateUpdate; 
        }
 
        private void OnDestroy()
        {
            GameManager.OnGameStateChanged -= HandleGameStateUpdate;
        }

        private void HandleGameStateUpdate(GameState gameState)
        {
            mainMenuStateGo?.SetActive(gameState == GameState.MainMenu);
            gameOverStateGo?.SetActive(gameState == GameState.GameOver);
            gameplayStateGo?.SetActive(gameState == GameState.GamePlay); 
            //pauseStateGo?.SetActive(gameState == GameState.Pause);
        }
 
        /// Shows the loading screen for at least the specified minimum time in seconds. 
        public static async Task ShowLoadingScreen(float minTimeToShow)
        {
            if (Instance == null)
            {
                Debug.LogError("LoadingScreenController Instance is not set!");
                return;
            }

            // Activate the loading screen
            Instance.loadingScreen.SetActive(true);

            // Wait for the minimum time (converted to milliseconds)
            await Task.Delay((int)(minTimeToShow * 1000));

            // Hide the loading screen
            Instance.loadingScreen.SetActive(false);
        }
    }
}