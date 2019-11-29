using System.IO;

namespace NeonTDS
{
    public class ClockMessage : Message
    {
        public uint Tick { get; set; }
        
        public ClockMessage()
            : base(MessageTypes.Clock)
        { }

        public override void FromBytes(BinaryReader reader)
        {
            base.FromBytes(reader);
            Tick = reader.ReadUInt32();
        }

        public override void ToBytes(BinaryWriter writer)
        {
            base.ToBytes(writer);
            writer.Write(Tick);
        }
    }
}
