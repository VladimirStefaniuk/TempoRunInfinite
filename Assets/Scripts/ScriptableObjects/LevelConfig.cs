using UnityEngine;
using UnityEngine.AddressableAssets; 

namespace Configs
{
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "ScriptableObjects/LevelConfig")]
    public class LevelConfig : ScriptableObject
    {
        public int numberOfTracks = 3;
        public float trackWidth = 1f;
        [Range(0.1f, 10f)]
        public float rowLength = 1f;

        public AssetReference alwaysFirstSegment;
        
        public AssetReference[] segments;  

        public AssetReference[] obstacles; 

        [Header("Coins")]
        public AssetReference coinPrefab; 
        public int minSequenceOfCoins = 3;
        public int maxSequenceOfCoins = 10;
        
        /// <summary>
        /// Indexed from 0, ..., N, where N is numberOfTracks - 1
        /// </summary>
        public int startingTrackIndex = 1;
        
        [Header("Preloading logic")]
        [Range(3, 50)]
        public int maxLoadedSegments = 5; 
    }
}