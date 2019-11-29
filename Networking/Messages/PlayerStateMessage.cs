using System.IO;
using System.Numerics;

namespace NeonTDS
{
    public class PlayerStateMessage : Message
    {
        public uint PlayerID { get; set; }
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
            PlayerID = reader.ReadUInt32();
            Position = new Vector2(reader.ReadSingle(), reader.ReadSingle());
            Direction = reader.ReadSingle();
            Speed = reader.ReadSingle();
            TurretDirection = reader.ReadSingle();
        }

        public override void ToBytes(BinaryWriter writer)
        {
            base.ToBytes(writer);
            writer.Write(PlayerID);
            writer.Write(Position.X);
            writer.Write(Position.Y);
            writer.Write(Direction);
            writer.Write(Speed);
            writer.Write(TurretDirection);
        }
    }
}
