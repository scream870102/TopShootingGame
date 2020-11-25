using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Eccentric;
using SgUnity.Player;
using SgUnity.Enemy;
namespace SgUnity
{
    class UIController : MonoBehaviour
    {
        int score = 0;
        [SerializeField] Slider playerHPSlider = null;
        [SerializeField] Text hpText = null;
        [SerializeField] Text scoreText = null;
        void OnEnable() {
            DomainEvents.Register<OnPlayerHPChange>(HandlePlayerHPChange);
            DomainEvents.Register<OnPlayerDead>(HandlePlayerDead);
            DomainEvents.Register<OnEnemyDead>(HandleEnemyDead);
        }
        void OnDisable() {
            DomainEvents.UnRegister<OnPlayerHPChange>(HandlePlayerHPChange);
            DomainEvents.UnRegister<OnPlayerDead>(HandlePlayerDead);
            DomainEvents.UnRegister<OnEnemyDead>(HandleEnemyDead);
        }

        void HandlePlayerHPChange(OnPlayerHPChange e) => playerHPSlider.value = e.HP;

        void HandlePlayerDead(OnPlayerDead e) {
            playerHPSlider.gameObject.SetActive(false);
            hpText.text = "Die die die";
        }

        void HandleEnemyDead(OnEnemyDead e) {
            score += e.Score;
            scoreText.text = e.Score.ToString();
        }
    }

}
