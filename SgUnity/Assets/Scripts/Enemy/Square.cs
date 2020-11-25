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
            components.Add(new SquareMove(PosList, attr, this));
            //components.Add(new TriangleShoot(attr, this));
        }

        void Update() {
            foreach (AEnemyComponent o in components)
                o.Tick();
        }
        public void SetAttribute(SquareAttribute attr) {
            this.attr = attr;
            Init(this.attr as BasicEnemyAttribute);
            foreach (AEnemyComponent o in components)
                (o as SquareComponent).Attr = this.attr;
        }
    }
    abstract class SquareComponent : AEnemyComponent
    {
        public SquareComponent(SquareAttribute attr, AEnemy parent) : base(parent) => this.Attr = attr;
        public SquareAttribute Attr { get; set; }
    }

    class SquareMove : SquareComponent
    {
        List<Transform> posList = new List<Transform>();
        ScaledTimer timer = null;
        Queue<int> posQueue = new Queue<int>();
        public SquareMove(List<Transform> posList, SquareAttribute attr, AEnemy parent) : base(attr, parent) {
            timer = new ScaledTimer(attr.PosInterval);
            posList.AddRange(posList);
        }

        ~SquareMove() { }
        public override void HandleEnable() {
            timer.Reset(this.Attr.PosInterval);
            posQueue.Clear();
            foreach (int i in Attr.PosQueue)
                posQueue.Enqueue(i);
        }

        public override void HandleDisable() { }

        public override void Tick() {
            if (timer.IsFinished && posQueue.Count != 0)
            {
                Parent.transform.position = posList[posQueue.Dequeue()].position;
                timer.Reset(Attr.PosInterval);
            }
        }
    }

    [System.Serializable]
    class SquareAttribute : BasicEnemyAttribute
    {
        public Queue<int> PosQueue = new Queue<int>(new[] { 0, 1, 2, 3 });
        public float PosInterval = 2.0f;
        public float ShootCd = 1f;
        public float BulletVel = 3f;
        public int Damage = 10;

    }
}
