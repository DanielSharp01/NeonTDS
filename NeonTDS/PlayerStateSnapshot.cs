using System.Collections.Generic;
using System.Numerics;

namespace NeonTDS
{
    public class PlayerStateSnapshot
    {
        public Vector2 Position { get; set; }
        public float Direction { get; set; }
        public float Speed { get; set; }

        public PlayerStateSnapshot(Vector2 position, float direction, float speed)
        {
            Position = position;
            Direction = direction;
            Speed = speed;
        }
    }

    public class PlayerStateSnapshotResolver
    {
        private (Vector2 position, float direction, float speed) absoluteState;
        private uint topSequenceNumber = 0;
        private List<(uint sequenceNumber, Vector2 position, float direction, float speed)> deltas = new List<(uint, Vector2, float, float)>();
        public PlayerStateSnapshot GetSnapshotForSequenceNumber(uint snapshotSequenceNumber)
        {
            (Vector2 position, float direction, float speed) state = absoluteState;
            foreach ((uint sequenceNumber, Vector2 position, float direction, float speed) in deltas)
            {
                if (sequenceNumber <= snapshotSequenceNumber)
                {
                    state = (state.position + position, state.direction + direction, state.speed + speed);
                }
            }
            return new PlayerStateSnapshot(state.position, state.direction, state.speed);
        }

        public void TakeSnapshot(uint sequenceNumber, Player player)
        {
            PlayerStateSnapshot lastSnapshot = GetSnapshotForSequenceNumber(sequenceNumber - 1);
            deltas.Add((sequenceNumber, player.Position - lastSnapshot.Position, player.Direction - lastSnapshot.Direction, player.Speed - lastSnapshot.Speed));
        }

        public void SetAbsoluteSnapshot(uint sequenceNumber, Player player)
        {
            deltas.RemoveRange(0, (int)(topSequenceNumber - topSequenceNumber));
            absoluteState = (player.Position, player.Direction, player.Speed);
            topSequenceNumber = sequenceNumber;
        }

        public void SetAbsoluteSnapshot(uint sequenceNumber, PlayerStateMessage message)
        {
            deltas.RemoveRange(0, (int)(topSequenceNumber - topSequenceNumber));
            absoluteState = (message.Position, message.Direction, message.Speed);
            topSequenceNumber = sequenceNumber;
        }
    }
}
