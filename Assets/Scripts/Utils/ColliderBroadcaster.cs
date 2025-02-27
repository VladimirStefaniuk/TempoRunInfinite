using UnityEngine;
using UnityEngine.Events;

namespace Utils
{
    public class ColliderBroadcaster : MonoBehaviour
    { 
        public UnityEvent<Collider> onTriggerEnter;
        public UnityEvent<Collider> onTriggerExit;
        public UnityEvent<Collider> onCollisionEnter;
        public UnityEvent<Collider> onCollisionExit;

        private void OnTriggerEnter(Collider other)
        {
            onTriggerEnter.Invoke(other);
        }
 
        private void OnTriggerExit(Collider other)
        {
            onTriggerExit.Invoke(other);
        }

        private void OnCollisionEnter(Collision collision)
        {
            onCollisionEnter.Invoke(collision.collider);
        }

        private void OnCollisionExit(Collision collision)
        {
            onCollisionExit.Invoke(collision.collider);
        }
    }
}