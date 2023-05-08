using System.Collections.Generic;
using System.Linq;
using Singleton;
using UnityEngine;

namespace ObjectPool
{
    [DefaultExecutionOrder(20)]
    public class PoolManager : Singleton<PoolManager>
    {
        [System.Serializable]
        public class ObjectPool
        {
            // Gameobject to pool
            public GameObject prefab;

            // Maximum instances of the gameobject
            public int maximumInstances;

            // Name of the pool
            public Pools.Types poolType;
            
            public Dictionary<int, GameObject> PassiveObjectsDictionary;

            [HideInInspector]
            public GameObject pool;

            /// <summary>
            /// Initialize the pool with creating instances of the gameobject and a container
            /// for the hierarchy
            /// </summary>
            public void InitializePool()
            {
                //activeList = new List<GameObject>();
                PassiveObjectsDictionary = new Dictionary<int, GameObject>();
                pool = new GameObject("[" + poolType + "]");
                //DontDestroyOnLoad(pool);

                // Reference to the created instance

                for (var i = 0; i < maximumInstances; i++)
                {
                    // Create the gameobject
                    var clone = Instantiate(prefab, pool.transform, true);

                    // Deactivate and add to the container and list
                    clone.SetActive(false);

                    PassiveObjectsDictionary.Add(clone.GetInstanceID(), clone);
                }
            }

            /// <summary>
            /// Get the next gameobject that can be spawned from the pool
            /// </summary>
            /// <returns>Next gameobject to spawn</returns>
            private GameObject _tempObject;
            public GameObject GetNextObject()
            {
                if (PassiveObjectsDictionary.Count > 0)
                {
                    _tempObject = PassiveObjectsDictionary.Values.ElementAt(0);
                    PassiveObjectsDictionary.Remove(PassiveObjectsDictionary.Keys.ElementAt(0));
                    return _tempObject;
                }
                else
                {
                    Debug.Log($"PoolManager: {PoolType} - passiveObjectsDictionary is empty. Instantiating new one.");
                    var clone = Instantiate(prefab, pool.transform, true);
                    clone.SetActive(false);
                    PassiveObjectsDictionary.Add(clone.GetInstanceID(), clone);

                    _tempObject = PassiveObjectsDictionary.Values.ElementAt(0);
                    PassiveObjectsDictionary.Remove(PassiveObjectsDictionary.Keys.ElementAt(0));
                    return _tempObject;
                }
            }

            // Properties
            public int MaximumInstances => maximumInstances;
            public Pools.Types PoolType { get => poolType;
                set => poolType = value;
            }
        }

        // List to hold all the pools for the game
        public List<ObjectPool> pools;
 
        protected void Awake()
        {
            
        
            // Initialize all the pools
            foreach (var t in pools)
            {
                t.InitializePool();
            }
        }

        /// <summary>
        /// Spawn the next gameobject at its current place 
        /// </summary>
        /// <param name="poolType"></param>
        /// <param name="parent"></param>
        /// <returns>Spawned gameobject</returns>
        public GameObject Spawn(Pools.Types poolType, Transform parent = null)
        {
            return Spawn(poolType, null, null, parent);
        }

        public GameObject Spawn(Pools.Types poolType, Vector3? position, Quaternion? rotation, Transform parent = null)
        {
            // Get the pool with the given pool name
            var pool = GetObjectPool(poolType);

            if (pool == null)
            {
                Debug.LogErrorFormat("Cannot find the object pool with name %s");

                return null;
            }

            // Get the next object from the pool
            var clone = pool.GetNextObject();

            if (clone == null)
            {
                //Debug.LogError("Scene contains maximum number of instances.");

                return null;
            }

            if (parent != null)
            {
                clone.transform.SetParent(parent);
            }

            if (position != null)
            {
                clone.transform.position = (Vector3)position;
            }

            if (rotation != null)
            {
                clone.transform.rotation = (Quaternion)rotation;
            }
            //Debug.Log(clone);
            // Spawn the gameobject
            clone.SetActive(true);

            //pool.activeList.Add(clone);

            //pool.passiveList.RemoveAt(pool.passiveList.Count - 1);

            return clone;
        }

        //public GameObject Spawn(Pools.Types poolType, Vector3 position, Quaternion rotation)
        //{
        //    // Spawn the gameobject
        //    GameObject clone = Spawn(poolType);

        //    // Set its position and rotation
        //    if (clone != null)
        //    {
        //        clone.transform.position = position;
        //        clone.transform.rotation = rotation;

        //        return clone;
        //    }

        //    return null;
        //}

        /// <summary>
        /// Spawn the next gameobject from the given pool to the random location between two
        /// vectors and given rotation
        /// </summary>
        /// <param name="poolType"></param>
        /// <param name="minVector">Minimum vector position for the spawned gameobject</param>
        /// <param name="maxVector">Maximum vector position for the spawned gameobject</param>
        /// <param name="rotation">Rotation of the spawned gameobject</param>
        /// <returns>Spawned gameobject</returns>
        public GameObject Spawn(Pools.Types poolType, Vector3 minVector, Vector3 maxVector, Quaternion rotation)
        {
            // Determine the random position
            var x = Random.Range(minVector.x, maxVector.x);
            var y = Random.Range(minVector.y, maxVector.y);
            var z = Random.Range(minVector.z, maxVector.z);
            var newPosition = new Vector3(x, y, z);

            // Spawn the next gameobject
            return Spawn(poolType, newPosition, rotation);
        }

        /// <summary>
        /// Despawn the given gameobject from the scene
        /// </summary>
        /// <param name="poolType"></param>
        /// <param name="obj">Gameobject to despawn</param>
        public void Despawn(Pools.Types poolType, GameObject obj)
        {
            var poolObject = GetObjectPool(poolType);

            obj.transform.SetParent(poolObject.pool.transform);

            //pool.activeList.Remove(obj);

            if (!poolObject.PassiveObjectsDictionary.ContainsKey(obj.GetInstanceID()))
            {
                poolObject.PassiveObjectsDictionary.Add(obj.GetInstanceID(), obj);
            }

            obj.SetActive(false);
        }

        /// <summary>
        /// Get the object pool reference from the pool list with the given pool name
        /// </summary>
        /// <param name="poolType"></param>
        /// <returns>ObjectPool object with the given name</returns>
        private ObjectPool GetObjectPool(Pools.Types poolType)
        {
            //var poolName = Pools.GetTypeStr(poolType);
            // Find the pool with the given name
            return pools.FirstOrDefault(t => t.PoolType == poolType);
        }

    }
}