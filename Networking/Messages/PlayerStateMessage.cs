using System.IO;
using System.Numerics;

namespace NeonTDS
{
    public class PlayerStateMessage : Message
    {
        public Vector2 Position { get; set; }
        public float Direction { get; set; }
        public float Speed { get; set; }
        public float TurretDirection { get; set; }
        public PlayerStateMessage()
            : base(MessageTypes.PlayerState)
        { }

        public override void FromBytes(BinaryReader reader)
        {
            base.FromBytes(reader);
            Position = new Vector2(reader.ReadSingle(), reader.ReadSingle());
            Direction = reader.ReadSingle();
            Speed = reader.ReadSingle();
            TurretDirection = reader.ReadSingle();
        }

        public override void ToBytes(BinaryWriter writer)
        {
            base.ToBytes(writer);
            writer.Write(Position.X);
            writer.Write(Position.Y);
            writer.Write(Direction);
            writer.Write(Speed);
            writer.Write(TurretDirection);
        }
    }
}
