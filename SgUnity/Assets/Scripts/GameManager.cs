using Eccentric.Utils;
using P = SgUnity.Player;
using UnityEngine;
using Cinemachine;
using System.Collections;
using SgUnity.Enemy.Boss;
using UnityEngine.SceneManagement;
using Eccentric;
namespace SgUnity
{
    class GameManager : TSingletonMonoBehavior<GameManager>
    {
        [SerializeField] float amplitude = 0f;
        [SerializeField] float frequency = 0f;
        bool bBossDie = false;
        bool bPlayerDie = false;
        bool bReload = false;
        CinemachineVirtualCamera cam = null;
        CinemachineBasicMultiChannelPerlin noise = null;
        public P.Player Player { get; private set; } = null;
        protected override void Awake()
        {
            base.Awake();
            Init();
        }


        void Init()
        {
            bBossDie = false;
            bPlayerDie = false;
            bReload = false;
            Player = GameObject.Find("Player").GetComponent<P.Player>();
            cam = GameObject.Find("vcam").GetComponent<CinemachineVirtualCamera>();
            noise = cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }

        void OnEnable()
        {
            DomainEvents.Register<OnBossDie>(HandleBossDie);
            DomainEvents.Register<P.OnPlayerDead>(HandlePlayerDead);
        }
        void OnDisable()
        {
            DomainEvents.UnRegister<OnBossDie>(HandleBossDie);
            DomainEvents.UnRegister<P.OnPlayerDead>(HandlePlayerDead);

        }

        void Update()
        {
            if (Player == null)
                Player = GameObject.Find("Player").GetComponent<P.Player>();
            if (cam == null)
            {
                cam = GameObject.Find("vcam").GetComponent<CinemachineVirtualCamera>();
                noise = cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            }
            if (bReload)
                Init();
            if (bBossDie || bPlayerDie)
            {
                if (Input.GetButtonDown("Shoot"))
                {
                    bBossDie = false;
                    bPlayerDie = false;
                    SceneManager.LoadScene(0);
                }
            }

        }

        public void StartCameraShake(float elapsed) => StartCoroutine(CameraShake(elapsed));

        IEnumerator CameraShake(float elapsed)
        {
            noise.m_FrequencyGain = frequency;
            noise.m_AmplitudeGain = amplitude;
            yield return new WaitForSeconds(elapsed);
            noise.m_FrequencyGain = 0f;
            noise.m_AmplitudeGain = 0f;
        }

        void HandleBossDie(OnBossDie e) => bBossDie = true;
        void HandlePlayerDead(P.OnPlayerDead e) => bPlayerDie = true;


    }
}
