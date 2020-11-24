using System.Collections.Generic;
using UnityEngine;
using Eccentric;
using Eccentric.Utils;
namespace SgUnity.Player
{
    class Player : MonoBehaviour
    {
        [ReadOnly] [SerializeField] int hp = 100;
        [SerializeField] ShootAttribute shootAttr = null;
        [SerializeField] MoveAttribute moveAttribute = null;
        PlayerInput input = null;
        List<PlayerComponent> components = new List<PlayerComponent>();

        public Rigidbody2D Rb { get; private set; }
        void OnEnable()
        {
            input.GamePlay.Enable();
            DomainEvents.Register<OnBulletHit>(HandleBulletHit);
        }

        void OnDisable()
        {
            input.GamePlay.Disable();
            DomainEvents.UnRegister<OnBulletHit>(HandleBulletHit);
        }

        void Awake()
        {
            Rb = GetComponent<Rigidbody2D>();
            input = new PlayerInput();
            components.Add(new Shoot(shootAttr, this, input));
            components.Add(new Move(moveAttribute, this, input));

        }

        void Update()
        {
            foreach (PlayerComponent o in components)
                o.Tick();

        }
        void HandleBulletHit(OnBulletHit e)
        {
            if (e.Type != EBULLET_TYPE.ENEMY || e.ObjectHit != this.gameObject)
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
        public OnPlayerHPChange(int newHP)
        {
            this.HP = newHP < 0 ? 0 : newHP;
        }
    }

}