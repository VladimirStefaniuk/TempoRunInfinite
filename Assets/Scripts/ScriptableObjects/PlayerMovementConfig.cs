using UnityEngine; 

namespace Configs
{
    [CreateAssetMenu(fileName = "PlayerMovementConfig", menuName = "ScriptableObjects/Player Movement Config")]
    public class PlayerMovementConfig : ScriptableObject
    {
        [Header("Running")] 
        public float runSpeed = 10f;
        public AnimationCurve runAccelerationCurve;
        
        [Header("Jumping")]
        public float jumpHeight = 1f;
        public float jumpDuration = 0.4f;
        
        [Header("Change line")]
        public float changeLineSpeed = 1f;
        public float changeLineThreshold = 0.2f;
    }
}
