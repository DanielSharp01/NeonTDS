using System.IO;

namespace NeonTDS
{
    public class EntityDestroyMessage : Message
    {
        public uint Timestamp { get; set; }
        public uint EntityID { get; set; }
        public EntityDestroyMessage()
            : base(MessageTypes.EntityDestroy)
        { }

        public override void FromBytes(BinaryReader reader)
        {
            base.FromBytes(reader);
            Timestamp = reader.ReadUInt32();
            EntityID = reader.ReadUInt32();
        }

        public override void ToBytes(BinaryWriter writer)
        {
            base.ToBytes(writer);
            writer.Write(Timestamp);
            writer.Write(EntityID);
        }
    }
}
