// TODO:ToTriangle

using UnityEngine;
using Eccentric.Utils;
using Lean.Pool;
using System.Collections.Generic;
namespace SgUnity.Enemy
{
    class Square : AEnemy
    {
        [SerializeField] SquareAttribute attr = null;
        public List<Transform> PosList { get; set; } = new List<Transform>();
        void Awake() {
            Init(attr as BasicEnemyAttribute);
            components.Add(new SquareMove(attr, this));
            components.Add(new SquareShoot(attr, this));
        }

        void Update() {
            foreach (AEnemyComponent o in components)
                o.Tick();
        }
        public void SetAttribute(SquareAttribute attr, List<Transform> spawnPoint) {
            PosList = new List<Transform>(spawnPoint);
            (components[0] as SquareMove).SetPosList(PosList);
            this.attr = attr;
            Init(this.attr as BasicEnemyAttribute);
            foreach (AEnemyComponent o in components)
                (o as SquareComponent).Attr = this.attr;
        }
    }
    abstract class SquareComponent : AEnemyComponent
    {
        public SquareComponent(SquareAttribute attr, AEnemy parent) : base(parent) => Attr = attr;
        public SquareAttribute Attr { get; set; }
    }

    class SquareMove : SquareComponent
    {
        List<Transform> posList = new List<Transform>();
        ScaledTimer timer = null;
        Queue<int> posQueue = new Queue<int>();
        public SquareMove(SquareAttribute attr, AEnemy parent) : base(attr, parent) {
            timer = new ScaledTimer(attr.PosInterval);
        }

        ~SquareMove() { }
        public override void HandleEnable() {
            timer.Reset(Attr.PosInterval);
            posQueue.Clear();
            foreach (int i in Attr.PosList)
                posQueue.Enqueue(i);
        }

        public override void HandleDisable() { }

        public override void Tick() {
            if (timer.IsFinished && posQueue.Count == 0)
                LeanPool.Despawn(Parent.gameObject);
            if (timer.IsFinished && posQueue.Count != 0)
            {
                Parent.transform.position = posList[posQueue.Dequeue()].position;
                timer.Reset(Attr.PosInterval);
            }
        }
        public void SetPosList(List<Transform> spawnPoints) => posList = new List<Transform>(spawnPoints);
    }
    class SquareShoot : SquareComponent
    {
        ScaledTimer timer = null;
        public SquareShoot(SquareAttribute attr, AEnemy parent) : base(attr, parent) {
            timer = new ScaledTimer(attr.ShootCd);
        }

        public override void Tick() {
            if (timer.IsFinished)
            {
                timer.Reset(Attr.ShootCd);
                LeanPool.Spawn(Parent.BulletPrefab, Parent.transform.position, Quaternion.identity).GetComponent<Bullet>().Shoot(new Vector2(0f, -Attr.BulletVel), EBulletType.ENEMY, Attr.Damage);
                LeanPool.Spawn(Parent.BulletPrefab, Parent.transform.position, Quaternion.identity).GetComponent<Bullet>().Shoot(new Vector2(0f, Attr.BulletVel), EBulletType.ENEMY, Attr.Damage);
                LeanPool.Spawn(Parent.BulletPrefab, Parent.transform.position, Quaternion.identity).GetComponent<Bullet>().Shoot(new Vector2(-Attr.BulletVel, 0f), EBulletType.ENEMY, Attr.Damage);
                LeanPool.Spawn(Parent.BulletPrefab, Parent.transform.position, Quaternion.identity).GetComponent<Bullet>().Shoot(new Vector2(Attr.BulletVel, 0f), EBulletType.ENEMY, Attr.Damage);
            }
        }

        public override void HandleEnable() { }
        public override void HandleDisable() { }
    }

    [System.Serializable]
    class SquareAttribute : BasicEnemyAttribute
    {
        public List<int> PosList = new List<int>() { 0, 1, 2, 3 };
        public float PosInterval = 2.0f;
        public float ShootCd = 1f;
        public float BulletVel = 3f;
        public int Damage = 10;

    }
}
