using UnityEngine;

namespace UI
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private GameObject mainMenuStateGo;
        [SerializeField] private GameObject gameOverStateGo;
        [SerializeField] private GameObject pauseStateGo;
        [SerializeField] private GameObject gameplayStateGo;
 
        private void Awake()
        {
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
    }
}