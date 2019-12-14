using System.IO;

namespace NeonTDS
{
    public class HealthMessage : Message
    {
        public uint PlayerID { get; set; }
        public byte Health { get; set; }
        public byte Shield { get; set; }
        public HealthMessage()
            : base(MessageTypes.Health)
        { }

        public override void FromBytes(BinaryReader reader)
        {
            base.FromBytes(reader);
            PlayerID = reader.ReadUInt32();
            Health = reader.ReadByte();
            Shield = reader.ReadByte();
        }

        public override void ToBytes(BinaryWriter writer)
        {
            base.ToBytes(writer);
            writer.Write(PlayerID);
            writer.Write(Health);
            writer.Write(Shield);
        }
    }
}
