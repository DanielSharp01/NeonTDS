using System.Numerics;

namespace NeonTDS
{
    public class PlayerStateSnapshot
    {
        public Vector2 Position { get; set; }
        public float Direction { get; set; }
        public float Speed { get; set; }

        public PlayerStateSnapshot(Player player)
        {
            Position = player.Position;
            Direction = player.Direction;
            Speed = player.Speed;
        }
    }
}
