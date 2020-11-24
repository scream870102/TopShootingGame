using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Eccentric;
using SgUnity.Player;
namespace SgUnity
{
    class UIController : MonoBehaviour
    {
        [SerializeField] Slider playerHPSlider = null;
        [SerializeField] Text hpText = null;
        void OnEnable()
        {
            DomainEvents.Register<OnPlayerHPChange>(HandlePlayerHPChange);
            DomainEvents.Register<OnPlayerDead>(HandlePlayerDead);
        }
        void OnDisable()
        {
            DomainEvents.UnRegister<OnPlayerHPChange>(HandlePlayerHPChange);
            DomainEvents.UnRegister<OnPlayerDead>(HandlePlayerDead);
        }

        void HandlePlayerHPChange(OnPlayerHPChange e)
        {
            playerHPSlider.value = e.HP;
        }

        void HandlePlayerDead(OnPlayerDead e)
        {
            playerHPSlider.gameObject.SetActive(false);
            hpText.text = "Die die die";
        }
    }

}
