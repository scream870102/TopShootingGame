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
        void Awake()
        {
            Init(attr as BasicEnemyAttribute);
            rb = GetComponent<Rigidbody2D>();
            components.Add(new TriangleMove(attr, this));
            //components.Add(new TriangleShoot(attr, this));
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            foreach (AEnemyComponent o in components)
                o.HandleEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            foreach (AEnemyComponent o in components)
                o.HandleDisable();
        }

        void Update()
        {
            foreach (AEnemyComponent o in components)
                o.Tick();
        }
    }

    class TriangleMove : AEnemyComponent
    {
        TriangleAttribute attr = null;
        Rigidbody2D rb = null;
        bool bStartFromRight = false;
        Vector2 dir = default(Vector2);
        public TriangleMove(TriangleAttribute attr, AEnemy parent) : base(parent)
        {
            this.attr = attr;
            Triangle triangle = parent as Triangle;
            triangle.OnColEnter += HandleColEnter;
            rb = triangle.Rb;
        }

        ~TriangleMove()
        {
            (Parent as Triangle).OnColEnter -= HandleColEnter;
        }
        // public override void Tick()
        // {

        // }
        public override void HandleEnable()
        {
            bStartFromRight = attr.IsStartFromRight;
            Init();
        }

        public override void HandleDisable()
        {
        }


        void HandleColEnter(Collision2D other)
        {
            Debug.Log(other);
            if (other.gameObject.tag != "Wall")
                return;
            bStartFromRight = !bStartFromRight;
            Init();

        }

        void Init()
        {
            Render2D.ChangeDirection(bStartFromRight, Parent.transform, true);
            dir = bStartFromRight ? Vector2.left : Vector2.right;
            rb.velocity = dir * attr.MoveSpeed;
        }
    }


    // class TriangleShoot : AEnemyComponent
    // {
    //     ScaledTimer timer = null;
    //     TriangleAttribute attr = null;
    //     public TriangleShoot(TriangleAttribute attr, AEnemy parent) : base(parent)
    //     {
    //         this.attr = attr;
    //         timer = new ScaledTimer(attr.ShootCd);
    //     }

    //     public override void Tick()
    //     {
    //         if (timer.IsFinished)
    //         {
    //             timer.Reset();
    //             LeanPool.Spawn(attr.BulletPrefab, Parent.transform.position, Quaternion.identity).GetComponent<Bullet>().Shoot(new Vector2(0f, -attr.BulletVel), EBULLET_TYPE.ENEMY, attr.Damage);
    //         }
    //     }
    // }

    [System.Serializable]
    class TriangleAttribute : BasicEnemyAttribute
    {
        [SerializeField] float moveSpeed = 3f;
        [SerializeField] float shootCd = 1f;
        [SerializeField] GameObject bulletPrefab = null;
        [SerializeField] float bulletVel = 3f;
        [SerializeField] int damage = 10;
        [SerializeField] bool bStartFromRight = false;
        public float MoveSpeed => moveSpeed;
        public float ShootCd => shootCd;
        public GameObject BulletPrefab => bulletPrefab;
        public float BulletVel => bulletVel;
        public int Damage => damage;
        public bool IsStartFromRight => bStartFromRight;

    }
}
