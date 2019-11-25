using System.IO;

namespace NeonTDS
{
    public class ClockMessage : Message
    {
        public uint Timestamp { get; set; }
        
        public ClockMessage()
            : base(MessageTypes.Clock)
        { }

        public override void FromBytes(BinaryReader reader)
        {
            base.FromBytes(reader);
            Timestamp = reader.ReadUInt32();
        }

        public override void ToBytes(BinaryWriter writer)
        {
            base.ToBytes(writer);
            writer.Write(Timestamp);
        }
    }
}
