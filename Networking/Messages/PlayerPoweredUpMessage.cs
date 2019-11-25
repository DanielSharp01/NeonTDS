using System.IO;

namespace NeonTDS
{
    public class PlayerPoweredUpMessage : Message
    {
        public uint PlayerID { get; set; }
        public PowerUpTypes PowerUpType { get; set; }
        public PlayerPoweredUpMessage()
            : base(MessageTypes.PlayerPoweredUp)
        { }

        public override void FromBytes(BinaryReader reader)
        {
            base.FromBytes(reader);
            PlayerID = reader.ReadUInt32();
            PowerUpType = (PowerUpTypes)reader.ReadByte();
        }

        public override void ToBytes(BinaryWriter writer)
        {
            base.ToBytes(writer);
            writer.Write(PlayerID);
            writer.Write((byte)PowerUpType);
        }
    }
}
