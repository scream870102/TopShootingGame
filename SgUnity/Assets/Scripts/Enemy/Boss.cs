using UnityEngine;
using Lean.Pool;
using System.Collections.Generic;
using Eccentric.Utils;
using System.Linq;
using Eccentric;
namespace SgUnity.Enemy.Boss
{
    class Boss : AEnemy
    {
        [SerializeField] BossAttribute attr = null;
        [SerializeField] GameObject chaseBulletPrefab = null;
        public GameObject ChaseBulletPrefab => chaseBulletPrefab;
        public int MaxHP => attr.HP;
        bool bStage2 = false;
        void Awake() {
            Init(attr as BasicEnemyAttribute);
            components.Add(new BossAttack(attr, this));
            bStage2 = false;
        }

        void Update() {
            foreach (AEnemyComponent o in components)
                o.Tick();
        }

        public void SetAttribute(BossAttribute attr) {
            this.attr = attr;
            Init(this.attr as BasicEnemyAttribute);
            foreach (AEnemyComponent o in components)
                (o as BossComponent).Attr = this.attr;
        }

        protected override void HandleBulletHit(OnBulletHit e) {
            if (e.Type != EBulletType.PLAYER || e.ObjectHit != gameObject)
                return;
            hp -= e.Damage;
            hp = hp < 0 ? 0 : hp;
            if (!bStage2 && hp <= MaxHP / 2)
            {
                bStage2 = true;
                DomainEvents.Raise<OnBossStage2>(new OnBossStage2());
            }
            DomainEvents.Raise<OnBossHPChange>(new OnBossHPChange(hp));
            if (hp == 0)
            {
                DomainEvents.Raise<OnBossDie>(new OnBossDie());
                gameObject.SetActive(false);
            }
        }
    }
    abstract class BossComponent : AEnemyComponent
    {
        public BossComponent(BossAttribute attr, AEnemy parent) : base(parent) => Attr = attr;
        public BossAttribute Attr { get; set; }

    }


    class BossAttack : BossComponent
    {
        List<AAttack> attacks = new List<AAttack>();
        AAttack currentAttack = null;
        public BossAttack(BossAttribute attr, AEnemy parent) : base(attr, parent) {
            attacks.Add(new Shotgun(Attr.ShotgunAttr, this));
            attacks.Add(new Spiral(Attr.SpiralAttr, this));
        }

        public override void Tick() {
            // No action right now find a new attack and active it
            if (currentAttack == null)
            {
                //Find available
                List<AAttack> avaliableAttack = new List<AAttack>(attacks.Where(a => a.IsReady));
                currentAttack = ChooseAttack(avaliableAttack);
                currentAttack?.Init();
            }
            // if attck is finish end it
            else if (currentAttack.IsFinished)
            {
                currentAttack.End();
                currentAttack = null;
            }
            else
                currentAttack.Tick();

        }
        public override void HandleEnable() {
            DomainEvents.Register<OnBossStage2>(HandleBossStage2);
        }
        public override void HandleDisable() {
            DomainEvents.UnRegister<OnBossStage2>(HandleBossStage2);
        }

        void HandleBossStage2(OnBossStage2 e) {
            attacks.Add(new Chaser(Attr.ChaserAttr, this));
        }

        AAttack ChooseAttack(List<AAttack> avaliable) {
            int sum = 0;
            avaliable.ForEach(a => sum += a.Probability);
            int result = Random.Range(0, sum + 1);
            int tmp = 0;
            foreach (AAttack a in avaliable)
            {
                tmp += a.Probability;
                if (tmp >= result)
                    return a;
            }
            return null;
        }
    }
    class Spiral : AAttack
    {
        float vel = 0;
        float cd = 0f;
        float elapsed = 0f;
        float intervalDegree = 0f;
        ScaledTimer elapsedTimer = null;
        ScaledTimer cdTimer = null;
        float currenDegree = 90f;
        public Spiral(SpiralAttr attr, BossAttack controller, bool isReadyAtFirst = true) : base(attr, controller, isReadyAtFirst) {
            vel = attr.Vel;
            cd = attr.CD;
            elapsed = attr.Elapsed;
            intervalDegree = attr.IntervalDegree;
        }

        public override void Init() {
            base.Init();
            elapsedTimer = new ScaledTimer(elapsed, false);
            cdTimer = new ScaledTimer(cd);
        }

        public override void Tick() {
            if (elapsedTimer.IsFinished)
                IsFinished = true;
            if (cdTimer.IsFinished)
            {
                Shoot();
                cdTimer.Reset(cd);
            }
        }

        void Shoot() {
            LeanPool.Spawn(Controller.Parent.BulletPrefab, Controller.Parent.transform.position, Quaternion.identity).GetComponent<Bullet>().
                Shoot(Math.GetDirectionFromDeg(currenDegree) * vel, EBulletType.ENEMY, Damage);
            currenDegree += intervalDegree;
        }
    }

    [System.Serializable]
    class SpiralAttr : BasicAttackAttr
    {
        public float Vel = 1.5f;
        public float CD = 0.5f;
        public float Elapsed = 5f;
        public float IntervalDegree = 15f;
    }

    class Chaser : AAttack
    {
        float vel = 0;
        float cd = 0f;
        float elapsed = 0f;
        float chaseTime = 0f;
        ScaledTimer elapsedTimer = null;
        ScaledTimer cdTimer = null;
        public Chaser(ChaserAttr attr, BossAttack controller, bool isReadyAtFirst = true) : base(attr, controller, isReadyAtFirst) {
            vel = attr.Vel;
            cd = attr.CD;
            elapsed = attr.Elapsed;
            chaseTime = attr.ChaseTime;
        }

        public override void Init() {
            base.Init();
            elapsedTimer = new ScaledTimer(elapsed, false);
            cdTimer = new ScaledTimer(cd);
        }

        public override void Tick() {
            if (elapsedTimer.IsFinished)
                IsFinished = true;
            if (cdTimer.IsFinished)
            {
                Shoot();
                cdTimer.Reset(cd);
            }
        }

        void Shoot() {
            LeanPool.Spawn((Controller.Parent as Boss).ChaseBulletPrefab, Controller.Parent.transform.position, Quaternion.identity)
                .GetComponent<ChaserBullet>().Shoot(GameManager.Instance.Player.transform, chaseTime, vel, EBulletType.ENEMY, Damage);
        }
    }

    [System.Serializable]
    class ChaserAttr : BasicAttackAttr
    {
        public float Vel = 2f;
        public float CD = 0.3f;
        public float Elapsed = 1f;
        public float ChaseTime = 3f;
    }

    class Shotgun : AAttack
    {
        float vel = 0;
        float cd = 0f;
        float elapsed = 0f;
        float maxDegree = 0f;
        int numOfBullets = 0;
        float intervalDegree = 0f;
        ScaledTimer elapsedTimer = null;
        ScaledTimer cdTimer = null;
        public Shotgun(ShotgunAttr attr, BossAttack controller, bool isReadyAtFirst = true) : base(attr, controller, isReadyAtFirst) {
            vel = attr.Vel;
            cd = attr.CD;
            elapsed = attr.Elapsed;
            maxDegree = attr.Degree;
            numOfBullets = attr.NumOfBullets;
            intervalDegree = maxDegree / (numOfBullets - 1);
        }

        public override void Init() {
            base.Init();
            elapsedTimer = new ScaledTimer(elapsed, false);
            cdTimer = new ScaledTimer(cd);
        }

        public override void Tick() {
            if (elapsedTimer.IsFinished)
                IsFinished = true;
            if (cdTimer.IsFinished)
            {
                Shoot();
                cdTimer.Reset(cd);
            }
        }

        void Shoot() {
            LeanPool.Spawn(Controller.Parent.BulletPrefab, Controller.Parent.transform.position, Quaternion.identity).GetComponent<Bullet>().Shoot(new Vector2(0f, -vel), EBulletType.ENEMY, Damage);
            for (int i = 1; i <= numOfBullets / 2; i++)
            {
                LeanPool.Spawn(Controller.Parent.BulletPrefab, Controller.Parent.transform.position, Quaternion.identity).GetComponent<Bullet>().Shoot(Math.GetDirectionFromDeg(i * intervalDegree + 270f) * vel, EBulletType.ENEMY, Damage);
                LeanPool.Spawn(Controller.Parent.BulletPrefab, Controller.Parent.transform.position, Quaternion.identity).GetComponent<Bullet>().Shoot(Math.GetDirectionFromDeg(630f - (i * intervalDegree)) * vel, EBulletType.ENEMY, Damage);
            }
        }
    }

    [System.Serializable]
    class ShotgunAttr : BasicAttackAttr
    {
        public float Vel = 4f;
        public float CD = 1f;
        public float Elapsed = 3f;
        public float Degree = 60f;
        public int NumOfBullets = 5;
    }


    [System.Serializable]
    class BossAttribute : BasicEnemyAttribute
    {
        public ShotgunAttr ShotgunAttr = null;
        public ChaserAttr ChaserAttr = null;
        public SpiralAttr SpiralAttr = null;
    }

    abstract class AAttack
    {
        ScaledTimer intervalTimer = null;
        float interval = 0f;
        protected BossAttack Controller { get; private set; }
        public int Probability { get; private set; }
        public bool IsFinished { get; protected set; }
        public bool IsReady { get { return intervalTimer.IsFinished; } }
        protected int Damage { get; private set; }
        public virtual void Init() => IsFinished = false;
        public abstract void Tick();
        public virtual void End() => intervalTimer.Reset(interval);

        public AAttack(BasicAttackAttr attr, BossAttack controller, bool isReadyAtFirst = true) {
            Probability = attr.Probability;
            interval = attr.Interval;
            Damage = attr.Damage;
            intervalTimer = new ScaledTimer(interval, isReadyAtFirst);
            Controller = controller;
        }
    }

    [System.Serializable]
    abstract class BasicAttackAttr
    {
        public int Probability = 0;
        public float Interval = 0f;
        public int Damage = 0;
    }

    class OnBossHPChange : IDomainEvent
    {
        public int HP { get; private set; }
        public OnBossHPChange(int hp) => HP = hp;


    }

    class OnBossDie : IDomainEvent
    {
        public OnBossDie() { }
    }

    class OnBossStage2 : IDomainEvent
    {
        public OnBossStage2() { }
    }
}
