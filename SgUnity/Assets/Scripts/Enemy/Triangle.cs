// TODO:ToTriangle

using UnityEngine;
using Eccentric.Utils;
using Lean.Pool;
namespace SgUnity.Enemy
{
    class Triangle : AEnemy
    {
        [SerializeField] TriangleAttribute attr = null;
        Rigidbody2D rb = null;
        public Rigidbody2D Rb => rb;
        void Awake() {
            Init(attr as BasicEnemyAttribute);
            rb = GetComponent<Rigidbody2D>();
            components.Add(new TriangleMove(attr, this));
            components.Add(new TriangleShoot(attr, this));
        }

        void Update() {
            foreach (AEnemyComponent o in components)
                o.Tick();
        }
        public void SetAttribute(TriangleAttribute attr) {
            this.attr = attr;
            Init(this.attr as BasicEnemyAttribute);
            foreach (AEnemyComponent o in components)
                (o as TriangleComponent).Attr = this.attr;
        }
    }
    abstract class TriangleComponent : AEnemyComponent
    {
        public TriangleComponent(TriangleAttribute attr, AEnemy parent) : base(parent) => Attr = attr;
        public TriangleAttribute Attr { get; set; }
    }

    class TriangleMove : TriangleComponent
    {
        Rigidbody2D rb = null;
        bool bStartFromRight = false;
        Vector2 dir = default(Vector2);
        public TriangleMove(TriangleAttribute attr, AEnemy parent) : base(attr, parent) {
            Triangle triangle = parent as Triangle;
            triangle.OnTriEnter += HandleTriEnter;
            rb = triangle.Rb;
        }

        ~TriangleMove() {
            (Parent as Triangle).OnTriEnter -= HandleTriEnter;
        }

        public override void HandleEnable() {
            bStartFromRight = Attr.IsStartFromRight;
            Init();
        }

        public override void HandleDisable() { }
        public override void Tick() { }

        void HandleTriEnter(Collider2D other) {
            if (!other.gameObject.CompareTag("Wall"))
                return;
            bStartFromRight = !bStartFromRight;
            Init();

        }

        void Init() {
            Render2D.ChangeDirection(bStartFromRight, Parent.transform, true);
            dir = bStartFromRight ? Vector2.left : Vector2.right;
            rb.velocity = dir * Attr.MoveSpeed;
        }
    }


    class TriangleShoot : TriangleComponent
    {
        ScaledTimer timer = null;
        public TriangleShoot(TriangleAttribute attr, AEnemy parent) : base(attr, parent) {
            timer = new ScaledTimer(attr.ShootCd);
        }

        public override void Tick() {
            if (timer.IsFinished)
            {
                timer.Reset(Attr.ShootCd);
                LeanPool.Spawn(Parent.BulletPrefab, Parent.transform.position, Quaternion.identity).GetComponent<Bullet>().Shoot(new Vector2(0f, -Attr.BulletVel), EBulletType.ENEMY, Attr.Damage);
                LeanPool.Spawn(Parent.BulletPrefab, Parent.transform.position, Quaternion.identity).GetComponent<Bullet>().Shoot(new Vector2(0f, Attr.BulletVel), EBulletType.ENEMY, Attr.Damage);
            }
        }

        public override void HandleEnable() { }
        public override void HandleDisable() { }
    }

    [System.Serializable]
    class TriangleAttribute : BasicEnemyAttribute
    {
        public float MoveSpeed = 3f;
        public float ShootCd = 1f;
        public float BulletVel = 3f;
        public int Damage = 10;
        public bool IsStartFromRight = false;

    }
}
