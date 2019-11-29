using System.IO;

namespace NeonTDS
{
    public class EntityCreateMessage : Message
    {
        public uint Tick { get; set; }
        public uint EntityID { get; set; }
        public EntityData EntityData { get; set; }
        public EntityCreateMessage()
            : base(MessageTypes.EntityCreate)
        { }

        public override void FromBytes(BinaryReader reader)
        {
            base.FromBytes(reader);
            Tick = reader.ReadUInt32();
            EntityID = reader.ReadUInt32();
            EntityData = EntityData.GetFromBytes(reader);
        }

        public override void ToBytes(BinaryWriter writer)
        {
            base.ToBytes(writer);
            writer.Write(Tick);
            writer.Write(EntityID);
            EntityData.ToBytes(writer);
        }
    }
}
