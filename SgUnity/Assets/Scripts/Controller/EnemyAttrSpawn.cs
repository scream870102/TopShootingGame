using UnityEngine;
using SgUnity.Enemy;
using System.Text.RegularExpressions;
using System;
using System.IO;
using Eccentric.Utils;
using UnityEngine.UI;
using UnityEditor;
#if UNITY_EDITOR
namespace SgUnity
{
    class EnemyAttrSpawn : MonoBehaviour
    {
        [ReadOnly] public string PATH = "";
        [SerializeField] string TA_FILENAME = "NONE";
        [SerializeField] TriangleAttribute TA = null;
        [SerializeField] string SQ_FILENAME = "NONE";
        [SerializeField] SquareAttribute SQ = null;

        public void Reset() {
            TA_FILENAME = "NONE";
            SQ_FILENAME = "NONE";
            TA = new TriangleAttribute();
            SQ = new SquareAttribute();
        }

        public void Apply() {
            CheckDirectory(PATH);
            if (TA_FILENAME != "NONE")
            {
                string path = PATH + "/" + TA_FILENAME;

                if (!File.Exists(path))
                {
                    FileStream fs = new FileStream(path, FileMode.Create);
                    string fileContext = JsonUtility.ToJson(TA);
                    StreamWriter file = new StreamWriter(fs);
                    file.Write(fileContext);
                    file.Close();
                    Debug.Log($"{path} created");
                }
            }
            if (SQ_FILENAME != "NONE")
            {
                string path = PATH + "/" + SQ_FILENAME;

                if (!File.Exists(path))
                {
                    FileStream fs = new FileStream(path, FileMode.Create);
                    string fileContext = JsonUtility.ToJson(SQ);
                    StreamWriter file = new StreamWriter(fs);
                    file.Write(fileContext);
                    file.Close();
                    Debug.Log($"{path} created");
                }
            }
        }


        void CheckDirectory(string path) {
            // Check if directory exists, if not create it
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
    }

    [CustomEditor(typeof(EnemyAttrSpawn))]
    public class EnemyAttrSpawnEditor : Editor
    {
        public override void OnInspectorGUI() {
            DrawDefaultInspector();
            EnemyAttrSpawn myScript = (EnemyAttrSpawn)target;
            myScript.PATH = Application.dataPath;
            if (GUILayout.Button("Apply"))
                myScript.Apply();
            if (GUILayout.Button("Reset"))
                myScript.Reset();
        }
    }
}
#endif
