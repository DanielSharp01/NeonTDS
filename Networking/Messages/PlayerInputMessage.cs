using System;
using System.IO;

namespace NeonTDS
{
    public class PlayerInputMessage : Message
    {
        public static uint NextSequenceNumber = 0;
        [Flags]
        private enum InputFlags {
            Firing = 1,
            TurningLeft = 2,
            TurningRight = 4,
            SpeedingUp = 8,
            SlowingDown = 16
        }

        public uint SequenceNumber { get; set; }

        public bool Firing { get; set; }
        public TurnState TurnState { get; set; }
        public SpeedState SpeedState { get; set; }

        public float TurretDirection { get; set; }

        public PlayerInputMessage()
            : base(MessageTypes.PlayerInput)
        {
            SequenceNumber = NextSequenceNumber++;
        }

        public override void FromBytes(BinaryReader reader)
        {
            base.FromBytes(reader);
            SequenceNumber = reader.ReadUInt32();
            InputFlags flags = (InputFlags)reader.ReadByte();
            Firing = (flags & InputFlags.Firing) != 0;
            TurnState = (flags & InputFlags.TurningLeft) != 0 ? TurnState.Left : (flags & InputFlags.TurningRight) != 0 ? TurnState.Right : TurnState.None;
            SpeedState = (flags & InputFlags.SpeedingUp) != 0 ? SpeedState.SpeedUp : (flags & InputFlags.SlowingDown) != 0 ? SpeedState.SlowDown : SpeedState.None;
            TurretDirection = reader.ReadSingle();
        }

        public override void ToBytes(BinaryWriter writer)
        {
            base.ToBytes(writer);
            writer.Write(SequenceNumber);
            InputFlags flags = 0;
            if (Firing) flags |= InputFlags.Firing;
            if (TurnState == TurnState.Left) flags |= InputFlags.TurningLeft;
            if (TurnState == TurnState.Right) flags |= InputFlags.TurningRight;
            if (SpeedState == SpeedState.SpeedUp) flags |= InputFlags.SpeedingUp;
            if (SpeedState == SpeedState.SlowDown) flags |= InputFlags.SlowingDown;
            writer.Write((byte)flags);
            writer.Write(TurretDirection);
        }
    }
}
