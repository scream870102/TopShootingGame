using UnityEngine;
using Eccentric.Utils;
using Lean.Pool;
namespace SgUnity.Enemy
{
    class Diamond : AEnemy
    {
        [SerializeField] DiamondAttribute attr = null;
        Rigidbody2D rb = null;
        public Rigidbody2D Rb => rb;
        void Awake()
        {
            Init(attr as BasicEnemyAttribute);
            rb = GetComponent<Rigidbody2D>();
            components.Add(new DiamondMove(attr, this));
            components.Add(new DiamondShoot(attr, this));
        }

        void Update()
        {
            foreach (AEnemyComponent o in components)
                o.Tick();
        }
    }

    class DiamondMove : AEnemyComponent
    {
        DiamondAttribute attr = null;
        Vector2 targetPos = default(Vector2);
        Rigidbody2D rb = null;
        public DiamondMove(DiamondAttribute attr, AEnemy parent) : base(parent)
        {
            this.attr = attr;
            FindNewTarget();
            Diamond diamond = parent as Diamond;
            diamond.OnColEnter += HandleColEnter;
            rb = diamond.Rb;
        }

        ~DiamondMove()
        {
            Diamond diamond = Parent as Diamond;
            diamond.OnColEnter -= HandleColEnter;
        }

        public override void Tick()
        {
            if (((Vector2)Parent.transform.position - targetPos).sqrMagnitude < .1f)
                FindNewTarget();
            Vector2 dir = (targetPos - (Vector2)Parent.transform.position).normalized;
            rb.velocity = dir * attr.MoveSpeed;
        }

        void FindNewTarget() => targetPos = new Vector2(Random.Range(-attr.MoveRange, attr.MoveRange), Parent.transform.position.y);

        void HandleColEnter(Collision2D other) => FindNewTarget();
    }


    class DiamondShoot : AEnemyComponent
    {
        ScaledTimer timer = null;
        DiamondAttribute attr = null;
        public DiamondShoot(DiamondAttribute attr, AEnemy parent) : base(parent)
        {
            this.attr = attr;
            timer = new ScaledTimer(attr.ShootCd);
        }

        public override void Tick()
        {
            if (timer.IsFinished)
            {
                timer.Reset();
                LeanPool.Spawn(attr.BulletPrefab, Parent.transform.position, Quaternion.identity).GetComponent<Bullet>().Shoot(new Vector2(0f, -attr.BulletVel), EBULLET_TYPE.ENEMY, attr.Damage);
            }
        }
    }

    [System.Serializable]
    class DiamondAttribute : BasicEnemyAttribute
    {
        [SerializeField] float moveRange = 0f;
        [SerializeField] float moveSpeed = 3f;
        [SerializeField] float shootCd = 1f;
        [SerializeField] GameObject bulletPrefab = null;
        [SerializeField] float bulletVel = 3f;
        [SerializeField] int damage = 10;
        public float MoveRange => moveRange;
        public float MoveSpeed => moveSpeed;
        public float ShootCd => shootCd;
        public GameObject BulletPrefab => bulletPrefab;
        public float BulletVel => bulletVel;
        public int Damage => damage;
    }
}
