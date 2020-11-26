using UnityEngine;
using Eccentric;
using Lean.Pool;
using Eccentric.Utils;
namespace SgUnity
{
    class ChaserBullet : MonoBehaviour
    {
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
                    sr.color = new Color(1f, .49f, .64f);
                    break;
                case EBulletType.ENEMY:
                    gameObject.layer = LayerMask.NameToLayer("EnemyBullet");
                    sr.color = new Color(.70f, .25f, .83f);
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
            DomainEvents.Raise<OnBulletHit>(new OnBulletHit(other.gameObject, damage, type));
            LeanPool.Despawn(gameObject);
        }
    }
}
