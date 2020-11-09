using UnityEngine;

namespace Unbeetleble.Game
{
    public class HoleEntry
    {
        public Vector2 position;
        public Border.Facing facing;

        public HoleEntry(Vector2 position, Border.Facing facing)
        {
            this.position = position;
            this.facing = facing;
        }
    }
}