using System.IO;

namespace NeonTDS
{
    public class DamageMessage : Message
    {
        public uint PlayerID { get; set; }
        public byte Damage { get; set; }
        public byte Health { get; set; }
        public DamageMessage()
            : base(MessageTypes.Damage)
        { }

        public override void FromBytes(BinaryReader reader)
        {
            base.FromBytes(reader);
            PlayerID = reader.ReadUInt32();
            Damage = reader.ReadByte();
            Health = reader.ReadByte();
        }

        public override void ToBytes(BinaryWriter writer)
        {
            base.ToBytes(writer);
            writer.Write(PlayerID);
            writer.Write(Damage);
            writer.Write(Health);
        }
    }
}
