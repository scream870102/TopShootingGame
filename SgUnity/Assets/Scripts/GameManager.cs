using Eccentric.Utils;
using P = SgUnity.Player;
using UnityEngine;
namespace SgUnity
{
    class GameManager : TSingletonMonoBehavior<GameManager>
    {
        public P.Player Player { get; private set; } = null;
        void Start() {
            if (Player == null)
                Player = GameObject.Find("Player").GetComponent<P.Player>();
        }
    }
}
