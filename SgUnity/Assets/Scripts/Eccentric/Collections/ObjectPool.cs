using System.Collections.Generic;

using UnityEngine;
namespace Eccentric.Collections {
    [System.Serializable]
    public class ObjectPool {
        [Header ("ObjectPool")]
        /// <summary>which object to spawn</summary>
        public GameObject pooledObject;
        /// <summary>all poolObjects should belong to which transform</summary>
        public Transform poolParent;
        /// <summary>how many item can pool hold</summary>
        public int pooledAmount;
        /// <summary>if user ask an item and pool is empty now Can pool instaniate an new item</summary>
        public bool IsGrow;
        public bool IsAvailable { get { return poolObjects.Count > 0; } }
        // To Store all item
        Queue<IObjectPoolAble> poolObjects;

        /// <summary>use this to set NECESSARY data</summary>
        /// <remarks>Not necessary to use constructor  you can also set data through inspector</remarks>
        public ObjectPool (GameObject pooledObject, Transform poolParent, int pooledAmount = 1, bool IsGrow = true) {
            this.pooledObject = pooledObject;
            this.poolParent = poolParent;
            this.pooledAmount = pooledAmount;
            this.IsGrow = IsGrow;
            Init ( );
        }

        /// <summary>Spawn all objects according to pooledAmount</summary>
        /// <remarks>MUST CALL this method if you set data with inspector not constructor</remarks>
        public List<IObjectPoolAble> Init ( ) {
            poolObjects = new Queue<IObjectPoolAble> ( );
            for (int i = 0; i < pooledAmount; i++) {
                poolObjects.Enqueue (SpawnObject ( ));
            }
            return new List<IObjectPoolAble> (poolObjects);
        }

        /// <summary>Return IObjectPoolItem</summary>
        /// <remarks>Need to convert to type which user need
        /// e.g. Bullets inherit from Mono,IObjectPoolItem
        /// if we ask for a bullet we need to convert type to bullet</remarks>
        /// <param name="data">you can add some data to object when it call init</param>
        /// <example><code>Bullet bullet = Bullets.GetPooledObject ( ) as Bullet;</code></example>
        public IObjectPoolAble GetPooledObject<T> (T data) {
            if (poolObjects.Count != 0) {
                IObjectPoolAble item = poolObjects.Dequeue ( );
                item.Init<T> (data);
                item.gameObject.SetActive (true);
                return item;
            }
            if (IsGrow) {
                IObjectPoolAble item = SpawnObject ( );
                item.Init<T> (data);
                item.gameObject.SetActive (true);
                return item;
            }
            return null;
        }

        /// <summary>Recycle Object to Pooling again</summary>
        /// <param name="item">which item will be Recycle to ObjectPooling</param>
        public void RecycleObject (IObjectPoolAble item) {
            poolObjects.Enqueue (item);
            item.gameObject.SetActive (false);
        }

        // Spawn Object and set its pool change its parent to poolParent then disable it
        IObjectPoolAble SpawnObject ( ) {
            IObjectPoolAble item = GameObject.Instantiate (pooledObject, poolParent).GetComponent<IObjectPoolAble> ( );
            item.Pool = this;
            item.gameObject.SetActive (false);
            return item;
        }
    }
}
