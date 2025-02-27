using UnityEngine; 

[ExecuteInEditMode]
public class LevelSegment : MonoBehaviour
{
    [SerializeField] private Transform _entrance;
    [SerializeField] private Transform _exit;
    
    public Transform Entrance => _entrance;
    public Transform Exit => _exit;

    /// if is true, then LevelGenerator won't place random obstacles on the segment
    [Header("Obstacles")] 
    public bool isManuallyPrebuild = false;
    [Range(0, 1)] public float chanceOfCoins = 0.5f;
    [Range(0, 1)] public float chanceOfObstacle = 0.2f;
    
    private LevelGenerator _injectedParentLevelGenerator;

    public void Init(LevelGenerator levelGenerator) // TODO: this about this approch how to improve
    {
        _injectedParentLevelGenerator = levelGenerator;
    }
    
    public void HandleOnExitTriggerEntered(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _injectedParentLevelGenerator.HandleOnExitTriggerEntered();
        }
    }

    #region "Debug"

    private readonly float _debugTargetGizmoCubeSize = 0.2f;
    
    private void OnDrawGizmos()
    {
        if(_entrance)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawCube(_entrance.position, Vector3.one * _debugTargetGizmoCubeSize);
        }
        
        if(_exit)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(_exit.position, Vector3.one * _debugTargetGizmoCubeSize);
        }
    }

    #endregion "Debug"
}