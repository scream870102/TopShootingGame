using System.Collections.Generic;
using UnityEngine;
using Eccentric;
using Eccentric.Utils;
namespace SgUnity.Player
{
    class Player : MonoBehaviour
    {
        [SerializeField] int hp = 100;
        [SerializeField] ShootAttribute shootAttr = null;
        [SerializeField] MoveAttribute moveAttribute = null;
        List<PlayerComponent> components = new List<PlayerComponent>();

        public Rigidbody2D Rb { get; private set; }
        void OnEnable() => DomainEvents.Register<OnBulletHit>(HandleBulletHit);

        void OnDisable() => DomainEvents.UnRegister<OnBulletHit>(HandleBulletHit);

        void Awake() {
            Rb = GetComponent<Rigidbody2D>();
            Rb.velocity = Vector2.zero;
            components.Add(new Shoot(shootAttr, this));
            components.Add(new Move(moveAttribute, this));
        }

        void Start() {
            DomainEvents.Raise<OnPlayerHPInit>(new OnPlayerHPInit(hp));
        }

        void Update() {
            foreach (PlayerComponent o in components)
                o.Tick();
        }

        void HandleBulletHit(OnBulletHit e) {
            if (e.Type != EBulletType.ENEMY || e.ObjectHit != gameObject)
                return;
            hp -= e.Damage;
            hp = hp < 0 ? 0 : hp;
            DomainEvents.Raise<OnPlayerHPChange>(new OnPlayerHPChange(hp));
            if (hp == 0)
                DomainEvents.Raise<OnPlayerDead>(new OnPlayerDead());
        }

    }

    class OnPlayerDead : IDomainEvent
    {
        public OnPlayerDead() { }
    }

    class OnPlayerHPChange : IDomainEvent
    {
        public int HP { get; private set; }
        public OnPlayerHPChange(int newHP) => HP = newHP < 0 ? 0 : newHP;
    }

    class OnPlayerHPInit : IDomainEvent
    {
        public int MaxHP { get; private set; }
        public OnPlayerHPInit(int maxHP) => MaxHP = maxHP;
    }

}