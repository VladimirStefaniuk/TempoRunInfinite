using UnityEngine;
using UnityEngine.Serialization;

namespace Configs
{
    [CreateAssetMenu(fileName = "PlayerMovementConfig", menuName = "ScriptableObjects/Player Movement Config")]
    public class PlayerMovementConfig : ScriptableObject
    {
        [Header("Running")] 
        public float runSpeed = 10f; 

        [Header("Jumping")] 
        public float jumpSwipeUpThreshold = 0.3f;
        public float jumpHeight = 1f;
        public float jumpForwardMultiplier = 2f;
        public float jumpDuration = 0.4f;
        
        [Header("Change line")]
        public float changeLineSpeed = 1f; 
        public float changeLineTimeout = 0.3f;
        public float inputMovementThreshold = 0.9f;
    }
}
