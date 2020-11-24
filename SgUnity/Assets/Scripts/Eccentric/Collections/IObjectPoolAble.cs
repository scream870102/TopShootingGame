using UnityEngine;
namespace Eccentric.Collections {
    /// <summary>class inherit this interface can be managed by class ObjectPool</summary>
    public interface IObjectPoolAble {
        //which pool should item belongs to 
        ObjectPool Pool { get; set; }
        //store ref for item's gameObject
        GameObject gameObject { get; }
        //recycle item to pool
        void Recycle ( );
        //action will do when item being push to pool
        void Init<T> (T data);
    }
}
