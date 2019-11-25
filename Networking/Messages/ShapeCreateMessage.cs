using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace NeonTDS
{
    public class ShapeCreateMessage : Message
    {
        public byte ShapeID { get; set; }
        public List<Vector2> Points { get; set; }

        public ShapeCreateMessage()
            : base(MessageTypes.ShapeCreate)
        { }

        public override void FromBytes(BinaryReader reader)
        {
            base.FromBytes(reader);
            ShapeID = reader.ReadByte();
            ushort pointCount = reader.ReadUInt16();
            Points = new List<Vector2>(pointCount);
            for (ushort i = 0; i < pointCount; i++)
            {
                Points.Add(new Vector2(reader.ReadSingle(), reader.ReadSingle()));
            }
        }

        public override void ToBytes(BinaryWriter writer)
        {
            base.ToBytes(writer);
            writer.Write(ShapeID);
            writer.Write((ushort)Points.Count);
            foreach (Vector2 point in Points) {
                writer.Write(point.X);
                writer.Write(point.Y);
            }
        }
    }
}
