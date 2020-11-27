// TODO:ToTriangle

using UnityEngine;
using Eccentric.Utils;
using Eccentric;
using Lean.Pool;
using P = SgUnity.Player;
using System.Linq;
namespace SgUnity.Enemy
{
    class Hexagon : AEnemy
    {
        [SerializeField] HexagonAttribute attr = null;
        Rigidbody2D rb = null;
        P.Player player = null;
        public Rigidbody2D Rb => rb;
        public P.Player Player => player;
        void Awake() {
            Init(attr as BasicEnemyAttribute);
            rb = GetComponent<Rigidbody2D>();
            player = GameManager.Instance.Player;
            components.Add(new HexagonMove(attr, this));
            components.Add(new HexagonShoot(attr, this));
        }

        void Update() => components.ForEach(o => o.Tick());

        public void SetAttribute(HexagonAttribute attr) {
            this.attr = attr;
            Init(this.attr as BasicEnemyAttribute);
            components.ForEach(o => (o as HexagonComponent).Attr = this.attr);
        }
    }

    abstract class HexagonComponent : AEnemyComponent
    {
        public HexagonComponent(HexagonAttribute attr, AEnemy parent) : base(parent) => Attr = attr;
        public HexagonAttribute Attr { get; set; }
    }

    class HexagonMove : HexagonComponent
    {
        Rigidbody2D rb = null;
        Vector2 dir = default(Vector2);
        P.Player player = null;
        public HexagonMove(HexagonAttribute attr, AEnemy parent) : base(attr, parent) {
            Hexagon hexagon = parent as Hexagon;
            rb = hexagon.Rb;
            player = hexagon.Player;
        }

        ~HexagonMove() { }
        public override void HandleEnable() { }
        public override void HandleDisable() { }

        public override void Tick() {
            Parent.transform.up = (player.transform.position - Parent.transform.position).normalized;
            rb.velocity = Parent.transform.up * Attr.MoveSpeed;
        }

    }


    class HexagonShoot : HexagonComponent
    {
        ScaledTimer timer = null;
        public HexagonShoot(HexagonAttribute attr, AEnemy parent) : base(attr, parent) => timer = new ScaledTimer(attr.ShootCd);

        public override void Tick() {
            if (timer.IsFinished)
            {
                timer.Reset(Attr.ShootCd);
                LeanPool.Spawn(Parent.BulletPrefab, Parent.transform.position, Quaternion.identity).GetComponent<Bullet>().Shoot(Parent.transform.up * Attr.BulletVel, EBulletType.ENEMY, Attr.Damage);
                float degree = Math.GetDegree(Parent.transform.up);
                for (int i = 1; i <= Attr.BulletNum / 2; i++)
                {
                    LeanPool.Spawn(Parent.BulletPrefab, Parent.transform.position, Quaternion.identity).GetComponent<Bullet>()
                    .Shoot(Math.GetDirectionFromDeg(degree + Attr.IntervalDegreeBetweenBullet * i) * Attr.BulletVel, EBulletType.ENEMY, Attr.Damage);
                    LeanPool.Spawn(Parent.BulletPrefab, Parent.transform.position, Quaternion.identity).GetComponent<Bullet>()
                    .Shoot(Math.GetDirectionFromDeg(degree - Attr.IntervalDegreeBetweenBullet * i) * Attr.BulletVel, EBulletType.ENEMY, Attr.Damage);
                }
            }
        }

        public override void HandleEnable() => timer.Reset(Attr.ShootCd);

        public override void HandleDisable() { }
    }

    [System.Serializable]
    class HexagonAttribute : BasicEnemyAttribute
    {
        public float MoveSpeed = 3f;
        public float ShootCd = 1f;
        public float BulletVel = 3f;
        public float IntervalDegreeBetweenBullet = 30f;
        public int BulletNum = 3;
        public int Damage = 10;

    }
}