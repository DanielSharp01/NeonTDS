using System.IO;

namespace NeonTDS
{
    public class ConnectResponseMessage : Message
    {
        public bool Approved { get; set; }
        public uint PlayerID { get; set; }
        public ConnectResponseMessage()
            : base(MessageTypes.ConnectResponse)
        { }

        public override void FromBytes(BinaryReader reader)
        {
            base.FromBytes(reader);
            Approved = reader.ReadBoolean();
            PlayerID = reader.ReadUInt32();
        }

        public override void ToBytes(BinaryWriter writer)
        {
            base.ToBytes(writer);
            writer.Write(Approved);
            writer.Write(PlayerID);
        }
    }
}
