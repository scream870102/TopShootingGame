using Eccentric.Utils;
using P = SgUnity.Player;
using UnityEngine;
using Cinemachine;
using System.Collections;
using SgUnity.Enemy;
using Lean.Pool;
namespace SgUnity
{
    class GameManager : TSingletonMonoBehavior<GameManager>
    {
        [SerializeField] CinemachineVirtualCamera cam = null;
        [SerializeField] float amplitude = 0f;
        [SerializeField] float frequency = 0f;
        [SerializeField] GameObject diePtc = null;
        [SerializeField] GameObject hitPtc = null;
        CinemachineBasicMultiChannelPerlin noise = null;
        public P.Player Player { get; private set; } = null;
        void Start() {
            if (Player == null)
                Player = GameObject.Find("Player").GetComponent<P.Player>();
            noise = cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }
        public void StartCameraShake(float elapsed) {
            StartCoroutine(CameraShake(elapsed));
        }

        IEnumerator CameraShake(float elapsed) {
            noise.m_FrequencyGain = frequency;
            noise.m_AmplitudeGain = amplitude;
            yield return new WaitForSeconds(elapsed);
            noise.m_FrequencyGain = 0f;
            noise.m_AmplitudeGain = 0f;
        }


        private void Update() {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("Shake");
                StartCameraShake(3f);
            }
        }

        void HandleEnemyDead(OnEnemyDead e) { }

    }
}
