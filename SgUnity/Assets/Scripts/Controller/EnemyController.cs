using UnityEngine;
using SgUnity.Enemy;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.IO;
using Lean.Pool;
namespace SgUnity
{
    class EnemyController : MonoBehaviour
    {
        [SerializeField] List<Transform> spawnPoint = new List<Transform>();
        [SerializeField] List<TriangleAttribute> triangleAttributes = new List<TriangleAttribute>();
        [SerializeField] List<GameObject> enemyPrefabs = new List<GameObject>();
        [SerializeField] List<GameObject> activeEnemies = new List<GameObject>();
        void Awake() {
            //add all spawnPoint to list
            for (int i = 0; i < transform.childCount; i++)
                spawnPoint.Add(transform.GetChild(i));
            GetAllAttributes();
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
                    break;
                case EEnemyType.DIAMOND:
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


}
