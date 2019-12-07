using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NeonTDS
{
    public class InputAckMessage : Message
    {
        public uint SequenceNumber { get; set; }
        public InputAckMessage()
            : base(MessageTypes.PlayerInputAck)
        { }

        public override void FromBytes(BinaryReader reader)
        {
            base.FromBytes(reader);
            SequenceNumber = reader.ReadUInt32();
        }

        public override void ToBytes(BinaryWriter writer)
        {
            base.ToBytes(writer);
            writer.Write(SequenceNumber);
        }
    }
}
