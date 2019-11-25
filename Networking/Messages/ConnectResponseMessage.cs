using System.IO;

namespace NeonTDS
{
    public class ConnectResponseMessage : Message
    {
        public bool Approved { get; set; }
        public ConnectResponseMessage()
            : base(MessageTypes.ConnectResponse)
        { }

        public override void FromBytes(BinaryReader reader)
        {
            base.FromBytes(reader);
            Approved = reader.ReadBoolean();
        }

        public override void ToBytes(BinaryWriter writer)
        {
            base.ToBytes(writer);
            writer.Write(Approved);
        }
    }
}
