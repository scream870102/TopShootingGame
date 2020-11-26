using UnityEngine;
using UnityEngine.UI;
using Eccentric;
using SgUnity.Player;
using SgUnity.Enemy;
using SgUnity.Enemy.Boss;
namespace SgUnity
{
    class UIController : MonoBehaviour
    {
        int score = 0;
        [SerializeField] Slider playerHPSlider = null;
        [SerializeField] Text hpText = null;
        [SerializeField] Text scoreText = null;
        [SerializeField] GameObject bossCanvas = null;
        [SerializeField] Slider bossHP = null;
        void OnEnable() {
            DomainEvents.Register<OnPlayerHPChange>(HandlePlayerHPChange);
            DomainEvents.Register<OnPlayerDead>(HandlePlayerDead);
            DomainEvents.Register<OnEnemyDead>(HandleEnemyDead);
            DomainEvents.Register<OnBossFightStart>(HandleBossFightStart);
            DomainEvents.Register<OnBossHPChange>(HandleBossHPChange);
            DomainEvents.Register<OnBossDie>(HandleBossDie);
            DomainEvents.Register<OnPlayerHPInit>(HandlePlayerHPInit);
        }
        void OnDisable() {
            DomainEvents.UnRegister<OnPlayerHPChange>(HandlePlayerHPChange);
            DomainEvents.UnRegister<OnPlayerDead>(HandlePlayerDead);
            DomainEvents.UnRegister<OnEnemyDead>(HandleEnemyDead);
            DomainEvents.UnRegister<OnBossFightStart>(HandleBossFightStart);
            DomainEvents.UnRegister<OnBossHPChange>(HandleBossHPChange);
            DomainEvents.UnRegister<OnBossDie>(HandleBossDie);
            DomainEvents.UnRegister<OnPlayerHPInit>(HandlePlayerHPInit);
        }

        void HandlePlayerHPChange(OnPlayerHPChange e) => playerHPSlider.value = e.HP;

        void HandlePlayerDead(OnPlayerDead e) {
            playerHPSlider.gameObject.SetActive(false);
            hpText.text = "Die die die";
        }

        void HandleEnemyDead(OnEnemyDead e) {
            score += e.Score;
            scoreText.text = score.ToString();
        }

        void HandleBossFightStart(OnBossFightStart e) {
            bossHP.maxValue = e.HP;
            bossHP.value = e.HP;
            bossCanvas.SetActive(true);
        }

        void HandleBossHPChange(OnBossHPChange e) {
            bossHP.value = e.HP;
        }

        void HandleBossDie(OnBossDie e) {
            hpText.text = "Win win win";
            bossCanvas.SetActive(false);
        }

        void HandlePlayerHPInit(OnPlayerHPInit e) {
            playerHPSlider.maxValue = e.MaxHP;
            playerHPSlider.value = e.MaxHP;
        }
    }

}
