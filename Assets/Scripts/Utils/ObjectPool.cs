using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
 
namespace Utils
{
    public class ObjectPool
    {
        private GameObject _prefab;
        private Transform _parent; 
        private Queue<GameObject> _pool = new ();
        private readonly object _syncLock = new object();  // Lock object for thread safety

        public ObjectPool(GameObject prefab, Transform parent)
        {
            _prefab = prefab;
            _parent = parent;
        }
  
        public async Task InitializeAsync(int size)
        { 
            for (int i = 0; i < size; i++)
            {
                Add();
                await Task.Yield();
            }
        }

        private void Add()
        {
            var go = Object.Instantiate(_prefab, _parent); 
            go.SetActive(false); 
            
            // Enqueue the object inside a lock to prevent concurrent modifications.
            lock (_syncLock)
            {
                _pool.Enqueue(go);
            } 
        }

        public GameObject Get()
        {
            GameObject go;
            lock (_syncLock)
            {
                if (_pool.Count == 0)
                {
                    Add();
                }
                go = _pool.Dequeue();
            } 
            return go;
        }

        public void Recycle(GameObject go)
        { 
            // Reset Transform and disable
            go.transform.SetParent(_parent);
            go.transform.position = Vector3.zero;
            go.transform.rotation = Quaternion.identity; 
            go.SetActive(false);
            
            // any script derived IPoolable, implements custom code to properly Recycle object
            IPoolable poolable = go.GetComponent<IPoolable>();
            if (poolable != null)
            {
                poolable.Recycle();  
            }
          
            lock (_syncLock)
            {
                _pool.Enqueue(go);
            }
        }
    }

}