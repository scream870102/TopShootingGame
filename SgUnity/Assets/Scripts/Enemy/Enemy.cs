using UnityEngine;
using System.Collections.Generic;
namespace SgUnity.Enemy
{
    class AEnemy : MonoBehaviour
    {
        public event System.Action<Collision2D> OnColEnter;
        public event System.Action<Collision2D> OnColStay;
        public event System.Action<Collision2D> OnColExit;
        protected List<AEnemyComponent> components = new List<AEnemyComponent>();
        void OnCollisionEnter2D(Collision2D other)
        {
            if (OnColEnter != null)
                OnColEnter(other);
        }
        void OnCollisionStay2D(Collision2D other)
        {
            if (OnColStay != null)
                OnColStay(other);
        }
        void OnCollisionExit2D(Collision2D other)
        {
            if (OnColExit != null)
                OnColExit(other);
        }
    }
    class AEnemyComponent
    {
        protected AEnemy Parent { get; private set; }
        public AEnemyComponent(AEnemy parent)
        {
            this.Parent = parent;
        }
        public virtual void Tick() { }

    }

}
