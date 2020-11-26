using UnityEngine;
using Eccentric.Utils;
using Lean.Pool;
namespace SgUnity.Player
{
    class Shoot : PlayerComponent
    {
        ScaledTimer timer = null;
        ShootAttribute attr = null;
        bool bShootPressed = false;
        public Shoot(ShootAttribute attr, Player player) : base(player) {
            this.attr = attr;
            timer = new ScaledTimer(attr.Cd);
        }


        public override void Tick() {
            if(Input.GetButtonDown("Shoot"))
                bShootPressed=true;
            else if(Input.GetButtonUp("Shoot"))
                bShootPressed=false;
            if (bShootPressed && timer.IsFinished)
            {
                timer.Reset();
                LeanPool.Spawn(attr.BulletPrefab, Player.transform.position, Quaternion.identity).GetComponent<Bullet>().Shoot(attr.BulletVelocity, EBulletType.PLAYER, attr.Damage);
            }
        }
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