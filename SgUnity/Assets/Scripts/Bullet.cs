using Lean.Pool;
using UnityEngine;
namespace SgUnity
{
    class Bullet : MonoBehaviour
    {
        Rigidbody2D rb = null;
        Collider2D col = null;
        SpriteRenderer sr = null;
        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            col = GetComponent<Collider2D>();
            sr = GetComponent<SpriteRenderer>();
            col.enabled = false;
        }

        public void Shoot(Vector2 vel, EBULLET_TYPE type, int damage = 0)
        {
            switch (type)
            {
                case EBULLET_TYPE.PLAYER:
                    this.gameObject.layer = LayerMask.NameToLayer("PlayerBullet");
                    sr.color = new Color(1f, .49f, .64f);
                    break;
                case EBULLET_TYPE.ENEMY:
                    this.gameObject.layer = LayerMask.NameToLayer("EnemyBullet");
                    sr.color = new Color(.16f, .61f, .41f);
                    break;
            }
            col.enabled = true;
            rb.velocity = vel;
        }

        void OnEnable() { }

        void OnDisable()
        {
            rb.velocity = Vector2.zero;
            col.enabled = false;
        }


        void OnCollisionEnter2D(Collision2D other)
        {
            LeanPool.Despawn(this.gameObject);
        }
    }
    enum EBULLET_TYPE
    {
        PLAYER,
        ENEMY
    }
}