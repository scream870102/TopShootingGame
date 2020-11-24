using System.Collections.Generic;
using UnityEngine;
namespace SgUnity.Player
{
    class Player : MonoBehaviour
    {
        [SerializeField] ShootAttribute shootAttr = null;
        [SerializeField] MoveAttribute moveAttribute = null;
        PlayerInput input = null;
        List<PlayerComponent> components = new List<PlayerComponent>();

        public Rigidbody2D Rb { get; private set; }
        void OnEnable()
        {
            input.GamePlay.Enable();
        }

        void OnDisable()
        {
            input.GamePlay.Disable();
        }

        void Awake()
        {
            Rb = GetComponent<Rigidbody2D>();
            input = new PlayerInput();
            components.Add(new Shoot(shootAttr, this, input));
            components.Add(new Move(moveAttribute, this, input));

        }

        void Update()
        {
            foreach (PlayerComponent o in components)
                o.Tick();

        }
    }

}