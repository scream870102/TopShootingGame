using UnityEngine;
using Eccentric;
using Lean.Pool;
using Eccentric.Utils;
namespace SgUnity
{
    class ChaserBullet : MonoBehaviour
    {
        [SerializeField] Color enemyColor = default(Color);
        [SerializeField] Color playerColor = default(Color);
        Rigidbody2D rb = null;
        Collider2D col = null;
        SpriteRenderer sr = null;
        EBulletType type = default(EBulletType);

        Transform target = null;
        float chaseTime = 0f;
        float vel = 0f;
        int damage = 0;
        ScaledTimer chaseTimer = null;
        void Awake() {
            rb = GetComponent<Rigidbody2D>();
            col = GetComponent<Collider2D>();
            sr = GetComponent<SpriteRenderer>();
            col.enabled = false;
        }

        void Update() {
            if (chaseTimer.IsFinished)
                LeanPool.Despawn(gameObject);
            if (target != null)
                rb.velocity = (Vector2)(target.position - transform.position).normalized * vel;
        }

        public void Shoot(Transform target, float chaseTime, float vel, EBulletType type, int damage = 0) {
            this.type = type;
            this.damage = damage;
            this.target = target;
            this.vel = vel;
            this.chaseTime = chaseTime;
            chaseTimer = new ScaledTimer(chaseTime, false);
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
        }

        void OnDisable() {
            rb.velocity = Vector2.zero;
            col.enabled = false;
            target = null;
        }

        void OnTriggerEnter2D(Collider2D other) {
            DomainEvents.Raise<OnBulletHit>(new OnBulletHit(other.gameObject, damage, type, transform.position));
            LeanPool.Despawn(gameObject);
        }
    }
}
