// TODO: Armor
// TODO: BOSS SHOTGUN M
using UnityEngine;
using Lean.Pool;
using System.Collections.Generic;
using Eccentric.Utils;
using System.Linq;
using System.IO;
using Eccentric;
namespace SgUnity.Enemy.Boss
{
    class Boss : AEnemy
    {
        [SerializeField] EnemyController enemyController = null;
        [SerializeField] BossAttribute attr = null;
        [SerializeField] GameObject chaseBulletPrefab = null;
        Rigidbody2D rb = null;
        public Rigidbody2D Rb => rb;
        public GameObject ChaseBulletPrefab => chaseBulletPrefab;
        public EnemyController EnemyController => enemyController;
        public int MaxHP => attr.HP;
        public bool IsMove
        {
            get { return (components[1] as BossMove).IsMove; }
            set { (components[1] as BossMove).IsMove = value; }
        }
        bool bStage2 = false;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            // #if UNITY_EDITOR
            //             //load all enemy setting from Application.DATAPATH 
            //             DirectoryInfo di = new DirectoryInfo(Application.persistentDataPath);
            //             FileInfo[] Files = di.GetFiles("BOSS.json");
            //             attr = JsonUtility.FromJson<BossAttribute>(Files[0].OpenText().ReadToEnd());
            // #endif
            Init(attr as BasicEnemyAttribute);
            components.Add(new BossAttack(attr, this));
            components.Add(new BossMove(attr, this));
            bStage2 = false;
        }

        void Update() => components.ForEach(o => o.Tick());

        public void SetAttribute(BossAttribute attr)
        {
            this.attr = attr;
            Init(this.attr as BasicEnemyAttribute);
            components.ForEach(o => (o as BossComponent).Attr = this.attr);
        }

        protected override void HandleBulletHit(OnBulletHit e)
        {
            if (e.Type != EBulletType.PLAYER || e.ObjectHit != gameObject)
                return;
            hp -= e.Damage;
            hp = hp < 0 ? 0 : hp;
            if (!bStage2 && hp <= MaxHP * .7f)
            {
                bStage2 = true;
                DomainEvents.Raise<OnBossStage2>(new OnBossStage2());
            }
            DomainEvents.Raise<OnBossHPChange>(new OnBossHPChange(hp));
            if (hp == 0)
            {
                LeanPool.Spawn(DiePtc, transform.position, Quaternion.identity).GetComponent<ParticleSystem>().Play();
                DomainEvents.Raise<OnBossDie>(new OnBossDie(attr.Score));
                gameObject.SetActive(false);
            }
            LeanPool.Spawn(HitPtc, e.PosHit, Quaternion.identity).GetComponent<ParticleSystem>().Play();
        }

    }
    abstract class BossComponent : AEnemyComponent
    {
        public BossComponent(BossAttribute attr, AEnemy parent) : base(parent) => Attr = attr;
        public BossAttribute Attr { get; set; }

    }
    class BossMove : BossComponent
    {
        float moveRange = 0f;
        float moveSpeed = 0f;
        float moveInterval = 0f;
        Vector2 initPos = new Vector2(-0.02f, 3.06f);
        ScaledTimer timer = null;
        public bool IsMove { get; set; }
        public BossMove(BossAttribute attr, AEnemy parent) : base(attr, parent)
        {
            moveRange = attr.MoveRange;
            moveSpeed = attr.MoveSpeed;
            moveInterval = attr.MoveInterval;
            timer = new ScaledTimer(moveInterval);
            IsMove = true;
        }

        public override void Tick()
        {
            if (!IsMove)
            {
                (Parent as Boss).Rb.velocity = Vector2.zero;
                return;
            }

            if (timer.IsFinished)
            {
                timer.Reset(moveInterval);
                Vector2 newPos = new Vector2(initPos.x + Random.Range(-moveRange, moveRange), initPos.y + Random.Range(-moveRange, 0));
                (Parent as Boss).Rb.velocity = (newPos - (Vector2)Parent.transform.position).normalized * moveSpeed;
            }
        }

        public override void HandleEnable() { }
        public override void HandleDisable() { }

    }

    class BossAttack : BossComponent
    {
        List<AAttack> attacks = new List<AAttack>();
        AAttack currentAttack = null;
        public BossAttack(BossAttribute attr, AEnemy parent) : base(attr, parent)
        {
            attacks.Add(new Shotgun(Attr.ShotgunAttr, this));
            attacks.Add(new Spiral(Attr.SpiralAttr, this));
            attacks.Add(new Callout(Attr.CalloutAttr, this));
        }

        public override void Tick()
        {
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
        public override void HandleEnable() => DomainEvents.Register<OnBossStage2>(HandleBossStage2);

        public override void HandleDisable() => DomainEvents.UnRegister<OnBossStage2>(HandleBossStage2);

        void HandleBossStage2(OnBossStage2 e)
        {
            attacks.Add(new Chaser(Attr.ChaserAttr, this));
            attacks.Add(new Wave(Attr.WaveAttr, this));
        }

        AAttack ChooseAttack(List<AAttack> avaliable)
        {
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

    class Callout : AAttack
    {
        int maxCommand = 0;
        List<string> commands = new List<string>();
        public Callout(CalloutAttr attr, BossAttack controller, bool isReadyAtFirst = true) : base(attr, controller, isReadyAtFirst)
        {
            maxCommand = attr.MaxCommand;
            commands.Clear();
            commands.AddRange(attr.Commands);
        }

        public override void Init()
        {
            base.Init();
            int num = Random.Range(2, maxCommand + 1);
            for (int i = 0; i < num; i++)
                (Controller.Parent as Boss).EnemyController.SpawnEnemy(commands[Random.Range(0, commands.Count)]);
            IsFinished = true;
        }

        public override void Tick() { }

    }

    [System.Serializable]
    class CalloutAttr : BasicAttackAttr
    {
        [Range(2, 20)]
        public int MaxCommand = 4;
        public List<string> Commands = new List<string>();
    }
    class Wave : AAttack
    {
        float vel = 0;
        float cd = 0f;
        float elapsed = 0f;
        float intervalDegree = 0f;
        float maxDegree = 0f;
        ScaledTimer elapsedTimer = null;
        ScaledTimer cdTimer = null;
        public Wave(WaveAttr attr, BossAttack controller, bool isReadyAtFirst = true) : base(attr, controller, isReadyAtFirst)
        {
            vel = attr.Vel;
            cd = attr.CD;
            elapsed = attr.Elapsed;
            intervalDegree = attr.IntervalDegree;
            maxDegree = attr.MaxDegree;
        }

        public override void Init()
        {
            base.Init();
            elapsedTimer = new ScaledTimer(elapsed, false);
            cdTimer = new ScaledTimer(cd);
        }

        public override void Tick()
        {
            if (elapsedTimer.IsFinished)
            {
                (Controller.Parent as Boss).IsMove = true;
                IsFinished = true;
            }
            if (cdTimer.IsFinished)
            {
                (Controller.Parent as Boss).IsMove = false;
                Shoot();
                cdTimer.Reset(cd);
            }
        }

        void Shoot()
        {
            float initDegree = Random.Range(270f, 630f);
            LeanPool.Spawn(Controller.Parent.BulletPrefab, Controller.Parent.transform.position, Quaternion.identity).GetComponent<Bullet>().
                Shoot(Math.GetDirectionFromDeg(initDegree) * vel, EBulletType.ENEMY, Damage);
            for (int i = 1; i <= maxDegree / intervalDegree; i++)
            {
                LeanPool.Spawn(Controller.Parent.BulletPrefab, Controller.Parent.transform.position, Quaternion.identity).GetComponent<Bullet>().
                    Shoot(Math.GetDirectionFromDeg(intervalDegree * -i + initDegree) * vel, EBulletType.ENEMY, Damage);
                LeanPool.Spawn(Controller.Parent.BulletPrefab, Controller.Parent.transform.position, Quaternion.identity).GetComponent<Bullet>().
                Shoot(Math.GetDirectionFromDeg(intervalDegree * i + initDegree) * vel, EBulletType.ENEMY, Damage);
            }
        }
    }

    [System.Serializable]
    class WaveAttr : BasicAttackAttr
    {
        public float Vel = 2f;
        public float CD = 2f;
        public float Elapsed = 6f;
        public float IntervalDegree = 20f;
        [Range(0f, 180f)]
        public float MaxDegree = 90f;
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
        public Spiral(SpiralAttr attr, BossAttack controller, bool isReadyAtFirst = true) : base(attr, controller, isReadyAtFirst)
        {
            vel = attr.Vel;
            cd = attr.CD;
            elapsed = attr.Elapsed;
            intervalDegree = attr.IntervalDegree;
        }

        public override void Init()
        {
            base.Init();
            elapsedTimer = new ScaledTimer(elapsed, false);
            cdTimer = new ScaledTimer(cd);
        }

        public override void Tick()
        {
            if (elapsedTimer.IsFinished)
                IsFinished = true;
            if (cdTimer.IsFinished)
            {
                Shoot();
                cdTimer.Reset(cd);
            }
        }

        void Shoot()
        {
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
        float damp = 0f;
        float chaseTime = 0f;
        ScaledTimer elapsedTimer = null;
        ScaledTimer cdTimer = null;
        public Chaser(ChaserAttr attr, BossAttack controller, bool isReadyAtFirst = true) : base(attr, controller, isReadyAtFirst)
        {
            vel = attr.Vel;
            cd = attr.CD;
            elapsed = attr.Elapsed;
            chaseTime = attr.ChaseTime;
            damp = attr.Damp;
        }

        public override void Init()
        {
            base.Init();
            elapsedTimer = new ScaledTimer(elapsed, false);
            cdTimer = new ScaledTimer(cd);
        }

        public override void Tick()
        {
            if (elapsedTimer.IsFinished)
                IsFinished = true;
            if (cdTimer.IsFinished)
            {
                Shoot();
                cdTimer.Reset(cd);
            }
        }

        void Shoot()
        {
            LeanPool.Spawn((Controller.Parent as Boss).ChaseBulletPrefab, Controller.Parent.transform.position, Quaternion.identity)
                .GetComponent<ChaserBullet>().Shoot(GameManager.Instance.Player.transform, chaseTime, vel, EBulletType.ENEMY, Damage, damp);
        }
    }

    [System.Serializable]
    class ChaserAttr : BasicAttackAttr
    {
        public float Vel = 3f;
        public float CD = 1f;
        public float Elapsed = 3f;
        public float ChaseTime = 4f;
        public float Damp = .1f;
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
        public Shotgun(ShotgunAttr attr, BossAttack controller, bool isReadyAtFirst = true) : base(attr, controller, isReadyAtFirst)
        {
            vel = attr.Vel;
            cd = attr.CD;
            elapsed = attr.Elapsed;
            maxDegree = attr.Degree;
            numOfBullets = attr.NumOfBullets;
            intervalDegree = maxDegree / (numOfBullets - 1);
        }

        public override void Init()
        {
            base.Init();
            elapsedTimer = new ScaledTimer(elapsed, false);
            cdTimer = new ScaledTimer(cd);
        }

        public override void Tick()
        {
            if (elapsedTimer.IsFinished)
            {
                (Controller.Parent as Boss).IsMove = true;
                IsFinished = true;
            }
            if (cdTimer.IsFinished)
            {
                (Controller.Parent as Boss).IsMove = false;
                Shoot();
                cdTimer.Reset(cd);
            }
        }

        void Shoot()
        {
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
        [Header("Move")]
        public float MoveRange = 3f;
        public float MoveSpeed = 3f;
        public float MoveInterval = 1f;
        [Header("Stage 1")]
        public ShotgunAttr ShotgunAttr = null;
        public SpiralAttr SpiralAttr = null;
        public CalloutAttr CalloutAttr = null;
        [Header("Stage 2")]
        public ChaserAttr ChaserAttr = null;
        public WaveAttr WaveAttr = null;
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

        public AAttack(BasicAttackAttr attr, BossAttack controller, bool isReadyAtFirst = true)
        {
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
        public int Score { get; private set; }
        public OnBossDie(int score) => Score = score;
    }

    class OnBossStage2 : IDomainEvent
    {
        public OnBossStage2() { }
    }
}
