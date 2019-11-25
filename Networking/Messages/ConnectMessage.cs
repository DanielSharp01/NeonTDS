using System.IO;

namespace NeonTDS
{
    public class ConnectMessage : Message
    {
        public string Name { get; set; }
        public byte Color { get; set; }
        public ConnectMessage()
            : base(MessageTypes.Connect)
        { }

        public override void FromBytes(BinaryReader reader)
        {
            base.FromBytes(reader);
            Name = reader.ReadString();
            Color = reader.ReadByte();
        }

        public override void ToBytes(BinaryWriter writer)
        {
            base.ToBytes(writer);
            writer.Write(Name);
            writer.Write(Color);
        }
    }
}
