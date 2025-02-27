using TMPro;
using UnityEngine;

namespace UI
{
    public class GameplayStateUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text distanceText;
        [SerializeField] private TMP_Text coinsText;
        public void OnEnable()
        {
            distanceText.text = UIHelper.FormatDistance(GameManager.Instance.DistanceRun);           
            coinsText.text = UIHelper.FormatCoins(GameManager.Instance.CoinsCollected);       
            
            GameManager.Instance.OnDistanceUpdated += UpdateDistanceUI;
            GameManager.Instance.OnCoinsUpdated += UpdateCoinsUI;
        }
        
        public void OnDisable()
        {
            GameManager.Instance.OnDistanceUpdated += UpdateDistanceUI;
            GameManager.Instance.OnCoinsUpdated += UpdateCoinsUI;
        }
        
        private void UpdateDistanceUI(float distance)
        {
            distanceText.text = UIHelper.FormatDistance(distance);    
        }

        private void UpdateCoinsUI(int coins)
        {
            coinsText.text = UIHelper.FormatCoins(coins);    
        } 
    }
}