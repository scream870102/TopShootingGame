using UnityEngine;
using SgUnity.Enemy;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.IO;
using Lean.Pool;
using Eccentric;
using SgUnity.Enemy.Boss;
using System.Linq;
namespace SgUnity
{
    class EnemyController : MonoBehaviour
    {
        [SerializeField] List<Transform> spawnPoint = new List<Transform>();
        [SerializeField] List<TriangleAttribute> triangleAttributes = new List<TriangleAttribute>();
        [SerializeField] List<SquareAttribute> squareAttributes = new List<SquareAttribute>();
        [SerializeField] List<HexagonAttribute> hexagonAttributes = new List<HexagonAttribute>();
        [SerializeField] List<GameObject> enemyPrefabs = new List<GameObject>();
        [SerializeField] List<GameObject> activeEnemies = new List<GameObject>();
        [SerializeField] Boss boss = null;
        void Awake() {
            //add all spawnPoint to list
            for (int i = 0; i < transform.childCount; i++)
                spawnPoint.Add(transform.GetChild(i));
            GetAllAttributes();
            boss.gameObject.SetActive(false);
        }

        void ActiveBossFight() {
            boss.gameObject.SetActive(true);
            DomainEvents.Raise<OnBossFightStart>(new OnBossFightStart(boss.MaxHP));
        }

        void SpawnEnemyAE(string s) {
            EnemySpawnEvent spawnEvent = new EnemySpawnEvent(s);
            GameObject o = LeanPool.Spawn(enemyPrefabs[(int)spawnEvent.type], spawnPoint[spawnEvent.startPosIndex].position, Quaternion.identity);
            o.SetActive(false);
            activeEnemies.Add(o);
            switch (spawnEvent.type)
            {
                case EEnemyType.TRIANGLE:
                    Triangle t = o.GetComponent<Triangle>();
                    t.SetAttribute(triangleAttributes[spawnEvent.settingIndex]);
                    o.SetActive(true);
                    break;
                case EEnemyType.SQUARE:
                    Square sq = o.GetComponent<Square>();
                    sq.PosList = new List<Transform>(spawnPoint);
                    sq.SetAttribute(squareAttributes[spawnEvent.settingIndex], spawnPoint);
                    o.SetActive(true);
                    break;
                case EEnemyType.HEXAGON:
                    Hexagon hex = o.GetComponent<Hexagon>();
                    hex.SetAttribute(hexagonAttributes[spawnEvent.settingIndex]);
                    o.SetActive(true);
                    break;
                case EEnemyType.BOSS:
                    break;
            }
        }

        void RecycleAllActiveEnemiesAE() {
            foreach (GameObject o in activeEnemies)
            {
                if (o.activeInHierarchy)
                    LeanPool.Despawn(o);
            }
        }

        void GetAllAttributes() {
            //load all enemy setting from Application.DATAPATH 
            DirectoryInfo di = new DirectoryInfo(Application.dataPath);
            FileInfo[] Files = di.GetFiles("*.json");
            foreach (FileInfo file in Files)
            {
                string type = file.Name.Split('_')[0];
                if (type == "TRIANGLE")
                    triangleAttributes.Add(JsonUtility.FromJson<TriangleAttribute>(file.OpenText().ReadToEnd()));
                else if (type == "SQUARE")
                    squareAttributes.Add(JsonUtility.FromJson<SquareAttribute>(file.OpenText().ReadToEnd()));
                else if (type == "HEXAGON")
                    hexagonAttributes.Add(JsonUtility.FromJson<HexagonAttribute>(file.OpenText().ReadToEnd()));

            }
        }
    }


    class EnemySpawnEvent
    {
        public EEnemyType type = default(EEnemyType);
        public int settingIndex = 0;
        public int startPosIndex = 0;

        public EnemySpawnEvent(string s) {
            string pattern = @"(\w+)\\+(\d+)\\+(\d+)";
            MatchCollection matches = Regex.Matches(s, pattern);
            Enum.TryParse<EEnemyType>(matches[0].Groups[1].Value, out type);
            settingIndex = int.Parse(matches[0].Groups[2].Value) - 1;
            startPosIndex = int.Parse(matches[0].Groups[3].Value) - 1;
        }

    }
    class OnBossFightStart : IDomainEvent
    {
        public int HP { get; private set; }
        public OnBossFightStart(int hp) => HP = hp;
    }


}
