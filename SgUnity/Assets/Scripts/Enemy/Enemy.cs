// TODO: Finish HandleBulletHit

using UnityEngine;
using System.Collections.Generic;
using Eccentric.Utils;
using Eccentric;
using Lean.Pool;
namespace SgUnity.Enemy
{
    abstract class AEnemy : MonoBehaviour
    {
        [SerializeField] GameObject bulletPrefab = null;
        public GameObject BulletPrefab => bulletPrefab;
        [ReadOnly] [SerializeField] protected int score = 0;
        [ReadOnly] [SerializeField] protected int hp = 0;
        public event System.Action<Collision2D> OnColEnter;
        public event System.Action<Collision2D> OnColStay;
        public event System.Action<Collision2D> OnColExit;
        public event System.Action<Collider2D> OnTriEnter;
        public event System.Action<Collider2D> OnTriStay;
        public event System.Action<Collider2D> OnTriExit;
        protected List<AEnemyComponent> components = new List<AEnemyComponent>();
        protected virtual void OnEnable() {
            DomainEvents.Register<OnBulletHit>(HandleBulletHit);
            foreach (AEnemyComponent o in components)
                o.HandleEnable();
        }

        protected virtual void OnDisable() {
            DomainEvents.UnRegister<OnBulletHit>(HandleBulletHit);
            foreach (AEnemyComponent o in components)
                o.HandleDisable();
        }

        void OnCollisionEnter2D(Collision2D other) {
            if (OnColEnter != null)
                OnColEnter(other);
        }
        void OnCollisionStay2D(Collision2D other) {
            if (OnColStay != null)
                OnColStay(other);
        }
        void OnCollisionExit2D(Collision2D other) {
            if (OnColExit != null)
                OnColExit(other);
        }

        void OnTriggerEnter2D(Collider2D other) {
            if (OnTriEnter != null)
                OnTriEnter(other);
        }

        void OnTriggerStay2D(Collider2D other) {
            if (OnTriStay != null)
                OnTriStay(other);
        }
        void OnTriggerExit2D(Collider2D other) {
            if (OnTriExit != null)
                OnTriExit(other);
        }

        protected void Init(BasicEnemyAttribute attr) {
            score = attr.Score;
            hp = attr.HP;
        }

        protected virtual void HandleBulletHit(OnBulletHit e) {
            if (e.Type != EBulletType.PLAYER || e.ObjectHit != gameObject)
                return;
            hp -= e.Damage;
            hp = hp < 0 ? 0 : hp;
            if (hp == 0)
            {
                LeanPool.Despawn(gameObject);
                DomainEvents.Raise<OnEnemyDead>(new OnEnemyDead(score, transform));
            }

        }
    }
    abstract class AEnemyComponent
    {
        public AEnemy Parent { get; private set; }
        public AEnemyComponent(AEnemy parent) => Parent = parent;
        public abstract void Tick();
        public abstract void HandleEnable();
        public abstract void HandleDisable();
    }

    class BasicEnemyAttribute
    {
        public int Score = 10;
        public int HP = 1;
    }

    enum EEnemyType
    {
        NONE,
        TRIANGLE,
        SQUARE,
        DIAMOND,
        BOSS,
    }

    class OnEnemyDead : IDomainEvent
    {
        public int Score { get; private set; }
        public Transform Transform { get; private set; }
        public OnEnemyDead(int score, Transform transform) {
            Score = score;
            Transform = transform;
        }
    }

}
