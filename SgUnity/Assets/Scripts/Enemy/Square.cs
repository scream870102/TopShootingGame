// TODO:ToTriangle

using UnityEngine;
using Eccentric.Utils;
using Lean.Pool;
using System.Collections.Generic;
using System.Linq;
namespace SgUnity.Enemy
{
    class Square : AEnemy
    {
        [SerializeField] SquareAttribute attr = null;
#if UNITY_EDITOR
        [SerializeField]

#endif
        List<Transform> posList = null;
        void Awake() {
            Init(attr as BasicEnemyAttribute);
            components.Add(new SquareMove(attr, this));
            components.Add(new SquareShoot(attr, this));
#if UNITY_EDITOR
            (components[0] as SquareMove).SetPosList(posList);
#endif
        }

        void Update() => components.ForEach(o => o.Tick());

        public void SetAttribute(SquareAttribute attr, List<Transform> spawnPoint) {
            posList = new List<Transform>(spawnPoint);
            (components[0] as SquareMove).SetPosList(posList);
            this.attr = attr;
            Init(this.attr as BasicEnemyAttribute);
            components.ForEach(o => (o as SquareComponent).Attr = this.attr);
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
        public SquareMove(SquareAttribute attr, AEnemy parent) : base(attr, parent) => timer = new ScaledTimer(attr.PosInterval);

        ~SquareMove() { }
        public override void HandleEnable() {
            timer.Reset(Attr.PosInterval);
            posQueue.Clear();
            Attr.PosList.ForEach(i => posQueue.Enqueue(i));
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
        public SquareShoot(SquareAttribute attr, AEnemy parent) : base(attr, parent) => timer = new ScaledTimer(attr.ShootCd);

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

        public override void HandleEnable() => timer.Reset(Attr.ShootCd);
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
