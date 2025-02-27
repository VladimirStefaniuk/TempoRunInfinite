using TMPro;
using UnityEngine;

namespace UI
{
    public class GameOverStateUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text distanceText;
        [SerializeField] private TMP_Text coinsText;

        public void OnEnable()
        {
            distanceText.text = UIHelper.FormatDistance(GameManager.Instance.DistanceRun);           
            coinsText.text = UIHelper.FormatCoins(GameManager.Instance.CoinsCollected);           
        } 
    }
}