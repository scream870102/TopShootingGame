// TODO: Finish HandleBulletHit
using UnityEngine;
using System.Collections.Generic;
using Eccentric.Utils;
using Eccentric;
namespace SgUnity.Enemy
{
    class AEnemy : MonoBehaviour
    {
        [ReadOnly] [SerializeField] protected int score = 0;
        [ReadOnly] [SerializeField] protected int hp = 0;
        public event System.Action<Collision2D> OnColEnter;
        public event System.Action<Collision2D> OnColStay;
        public event System.Action<Collision2D> OnColExit;
        protected List<AEnemyComponent> components = new List<AEnemyComponent>();
        protected virtual void OnEnable()
        {
            DomainEvents.Register<OnBulletHit>(HandleBulletHit);
        }

        protected virtual void OnDisable()
        {
            DomainEvents.UnRegister<OnBulletHit>(HandleBulletHit);
        }

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

        protected void Init(BasicEnemyAttribute attr)
        {
            this.score = attr.Score;
            this.hp = attr.HP;
        }

        protected virtual void HandleBulletHit(OnBulletHit e)
        {
            if (e.Type != EBULLET_TYPE.PLAYER || e.ObjectHit != this.gameObject)
                return;
            hp -= e.Damage;
            hp = hp < 0 ? 0 : hp;
            if (hp == 0)
            {
                gameObject.SetActive(false);
                DomainEvents.Raise<OnEnemyDead>(new OnEnemyDead(score));
            }

        }
    }
    class AEnemyComponent
    {
        protected AEnemy Parent { get; private set; }
        public AEnemyComponent(AEnemy parent) => this.Parent = parent;
        public virtual void Tick() { }
        public virtual void HandleEnable() { }
        public virtual void HandleDisable() { }
    }

    class BasicEnemyAttribute
    {
        [SerializeField] int score = 10;
        [SerializeField] int hp = 0;
        public int Score => score;
        public int HP => hp;
    }

    enum EEnemyType
    {
        DIAMOND,
        BOSS,
    }

    class OnEnemyDead : IDomainEvent
    {
        public int Score { get; private set; }
        public OnEnemyDead(int score) => this.Score = score;
    }

}
