using System; 
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Utils
{
    [Serializable]
    public class PoolConfig
    {
        public AssetReference prefab;
        public int size;
    }
    
    public class PoolManager : MonoBehaviour
    {
        public static PoolManager Instance { get; private set; }

        private Dictionary<string, ObjectPool> pools = new (); 
        
        private Dictionary<GameObject, ObjectPool> PoolLookup = new ();

        public List<PoolConfig> PoolConfigs;
        
        public bool IsInitialized { get; private set; }
        
        private void Awake()
        {
            DontDestroyOnLoad(this);
            if (!Instance)
                Instance = this;
            else
                Destroy(gameObject);
        }

        private async void Start()
        {
            try
            {
                await InitializeAllPoolsAsync();
                IsInitialized = true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error in {nameof(InitializePoolAsync)}: {ex}");
            }
        }


        private async Task InitializeAllPoolsAsync()
        {
            List<Task> initializationTasks = new();
            foreach (var pool in PoolConfigs)
            {
                initializationTasks.Add(InitializePoolAsync(pool.prefab, pool.size));
            }
            await Task.WhenAll(initializationTasks);
        }
        
        
        /// <summary>
        /// Loads the asset and creates a pool when it's ready.
        /// </summary>
        private async Task InitializePoolAsync(AssetReference assetReference, int size)
        {
            var handle = assetReference.LoadAssetAsync<GameObject>();
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                // create new go with name of the pool as child of PoolManager
                var poolParent = new GameObject($"POOL_{assetReference.Asset.name}").transform;
                poolParent.SetParent(transform);
                
                GameObject prefab = handle.Result;
                var pool = new ObjectPool(prefab, poolParent);
                pools[assetReference.AssetGUID] = pool;
                await pool.InitializeAsync(size);
            }
            else
            {
                Debug.LogError($"Failed to load Addressable: {assetReference.RuntimeKey}");
            }
        }

        public GameObject Get(AssetReference asset)
        {
            return Get(asset.AssetGUID);
        }
        
        public GameObject Get(string key)
        {
            if (!IsInitialized)
            {
                Debug.LogError("PoolManager is not initialized yet!");
                return null;
            }
            
            if (pools.ContainsKey(key))
            {
                var go = pools[key].Get();
                PoolLookup[go] = pools[key];
                return go;
            }

            Debug.LogError($"Pool with key '{key}' not found!");
            return null;
        }
 
        public void Recycle(GameObject go)
        {
            if (!PoolLookup.ContainsKey(go))
            {
                // If no pool exists, destroy it to prevent memory leaks.
                Destroy(go);
                Debug.LogError($"Recycle called for {go.name} but object is not registered! Properly register with PoolManager");
            } 
            PoolLookup[go].Recycle(go);  
        }
    }
}