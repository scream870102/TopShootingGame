using Lean.Pool;
using UnityEngine;
using Eccentric;
namespace SgUnity
{
    class Bullet : MonoBehaviour
    {
        [SerializeField] Color enemyColor = default(Color);
        [SerializeField] Color playerColor = default(Color);
        Rigidbody2D rb = null;
        Collider2D col = null;
        SpriteRenderer sr = null;
        EBulletType type = default(EBulletType);
        int damage = 0;
        void Awake() {
            rb = GetComponent<Rigidbody2D>();
            col = GetComponent<Collider2D>();
            sr = GetComponent<SpriteRenderer>();
            col.enabled = false;
        }

        public void Shoot(Vector2 vel, EBulletType type, int damage = 0) {
            this.type = type;
            this.damage = damage;
            switch (type)
            {
                case EBulletType.PLAYER:
                    gameObject.layer = LayerMask.NameToLayer("PlayerBullet");
                    sr.color = playerColor;
                    break;
                case EBulletType.ENEMY:
                    gameObject.layer = LayerMask.NameToLayer("EnemyBullet");
                    sr.color = enemyColor;
                    break;
            }
            col.enabled = true;
            rb.velocity = vel;
        }

        void OnDisable() {
            rb.velocity = Vector2.zero;
            col.enabled = false;
        }

        void OnTriggerEnter2D(Collider2D other) {
            DomainEvents.Raise<OnBulletHit>(new OnBulletHit(other.gameObject, damage, type, transform.position));
            LeanPool.Despawn(gameObject);
        }
    }

    enum EBulletType
    {
        PLAYER,
        ENEMY
    }

    class OnBulletHit : IDomainEvent
    {
        public GameObject ObjectHit { get; private set; }
        public int Damage { get; private set; }
        public EBulletType Type { get; private set; }
        public Vector3 PosHit { get; private set; }
        public OnBulletHit(GameObject objectHit, int damage, EBulletType type, Vector3 posHit) {
            ObjectHit = objectHit;
            Damage = damage;
            Type = type;
            PosHit = posHit;
        }
    }
}