using UnityEngine;

namespace SgUnity.Player
{
    class Move : PlayerComponent
    {
        Rigidbody2D rb = null;
        Vector2 direction = default(Vector2);
        MoveAttribute attr = null;
        public Move(MoveAttribute attr, Player player) : base(player) {
            this.attr = attr;
            rb = player?.Rb;
        }


        public override void Tick() {
            direction = Vector2.zero;
            direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
            rb.velocity = direction * attr.Vel;
        }

    }

    [System.Serializable]
    class MoveAttribute
    {
        [SerializeField] float vel = 1f;
        public float Vel => vel;
    }
}
