namespace SgUnity.Player
{
    abstract class PlayerComponent
    {
        protected Player Player { get; private set; }
        protected PlayerInput Input { get; private set; }
        public PlayerComponent(Player player, PlayerInput input) {
            this.Player = player;
            this.Input = input;
        }
        public virtual void Tick() { }


    }
}