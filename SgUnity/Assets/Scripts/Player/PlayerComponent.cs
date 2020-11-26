namespace SgUnity.Player
{
    abstract class PlayerComponent
    {
        protected Player Player { get; private set; }
        public PlayerComponent(Player player) => Player = player;
        public virtual void Tick() { }


    }
}