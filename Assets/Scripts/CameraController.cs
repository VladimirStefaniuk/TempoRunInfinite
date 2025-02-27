using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Vector3 offset = new(0f, 0f, -10f);
    [SerializeField] private float smoothTime = 0.25f; 
    [SerializeField] private Transform player;

    private void LateUpdate()
    {
        transform.position = new Vector3(
            player.transform.position.x + offset.x,
            transform.position.y,
            player.transform.position.z + offset.z);
    }
}