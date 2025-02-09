using UnityEngine;

namespace Dasverse.Aleo.UI
{
    interface IJoyStick
    {
        public HandType HandType { get; set; }
        public Vector2 JoyStickDirection { get; /*set;*/ }
        public void LocationSet(HandType handType, int offsetX = 50, int offsetY = 50);
    }
}
