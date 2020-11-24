using Lean.Pool;
using UnityEngine;
namespace SgUnity.Player
{
    class Bullet : MonoBehaviour
    {
        Rigidbody2D rb = null;
        Collider2D col = null;
        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            col = GetComponent<Collider2D>();
            col.enabled = false;
        }

        public void Shoot(Vector2 vel)
        {
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
}