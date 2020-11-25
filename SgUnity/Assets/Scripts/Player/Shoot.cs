using UnityEngine;
using UnityEngine.InputSystem;
using Eccentric.Utils;
using Lean.Pool;
namespace SgUnity.Player
{
    class Shoot : PlayerComponent
    {
        ScaledTimer timer = null;
        ShootAttribute attr = null;
        bool bShootPressed = false;
        public Shoot(ShootAttribute attr, Player player, PlayerInput input) : base(player, input) {
            this.Input.GamePlay.Shoot.started += HandleShootStarted;
            this.Input.GamePlay.Shoot.canceled += HandleShootCanceled;
            this.attr = attr;
            timer = new ScaledTimer(attr.Cd);
        }

        ~Shoot() {
            this.Input.GamePlay.Shoot.started -= HandleShootStarted;
            this.Input.GamePlay.Shoot.canceled -= HandleShootCanceled;
        }

        public override void Tick() {
            if (bShootPressed && timer.IsFinished)
            {
                timer.Reset();
                LeanPool.Spawn(attr.BulletPrefab, Player.transform.position, Quaternion.identity).GetComponent<Bullet>().Shoot(attr.BulletVelocity, EBulletType.PLAYER, attr.Damage);
            }
        }

        void HandleShootStarted(InputAction.CallbackContext ctx) => bShootPressed = true;

        void HandleShootCanceled(InputAction.CallbackContext ctx) => bShootPressed = false;

    }
    [System.Serializable]
    class ShootAttribute
    {
        [SerializeField] GameObject bulletPrefab = null;
        [SerializeField] float cd = 0.5f;
        [SerializeField] Vector2 bulletVelocity = new Vector2(0f, .5f);
        [SerializeField] int damage = 10;
        public GameObject BulletPrefab => bulletPrefab;
        public float Cd => cd;
        public Vector2 BulletVelocity => bulletVelocity;
        public int Damage => damage;
    }
}