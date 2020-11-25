using UnityEngine;
using UnityEngine.InputSystem;

namespace SgUnity.Player
{
    class Move : PlayerComponent
    {
        Rigidbody2D rb = null;
        Vector2 direction = default(Vector2);
        MoveAttribute attr = null;
        public Move(MoveAttribute attr, Player player, PlayerInput input) : base(player, input) {
            this.Input.GamePlay.Move.started += HandleMoveStarted;
            this.Input.GamePlay.Move.performed += HandleMovePerformed;
            this.Input.GamePlay.Move.canceled += HandleMoveCanceled;
            this.attr = attr;
            rb = player?.Rb;

        }

        ~Move() {
            this.Input.GamePlay.Move.started -= HandleMoveStarted;
            this.Input.GamePlay.Move.performed -= HandleMovePerformed;
            this.Input.GamePlay.Move.canceled -= HandleMoveCanceled;
        }

        public override void Tick() => rb.velocity = direction * attr.Vel;

        void HandleMoveStarted(InputAction.CallbackContext ctx) => direction = ctx.ReadValue<Vector2>();
        void HandleMovePerformed(InputAction.CallbackContext ctx) => direction = ctx.ReadValue<Vector2>();
        void HandleMoveCanceled(InputAction.CallbackContext ctx) => direction = Vector2.zero;
    }


    [System.Serializable]
    class MoveAttribute
    {
        [SerializeField] float vel = 1f;
        public float Vel => vel;
    }
}
