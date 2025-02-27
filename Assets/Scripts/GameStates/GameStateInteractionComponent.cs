using JetBrains.Annotations;
using UnityEngine;

namespace GameStates
{
    public class GameStateInteractionComponent : MonoBehaviour
    {
        [SerializeField] private GameState State;
        
        [UsedImplicitly]
        public void RequestGameStateUpdate()
        {
            GameManager.Instance.UpdateGameState(State);
        }
    }
}