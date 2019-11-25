using System.IO;
using System.Numerics;

namespace NeonTDS
{
    public class EntityData
    {
        EntityTypes Type { get; set; }

        public EntityData(EntityTypes type)
        {
            Type = type;
        }

        public static EntityData GetFromBytes(BinaryReader reader)
        {
            EntityData data = new EntityData(EntityTypes.Unknown);
            switch ((EntityTypes)reader.ReadByte())
            {
                case EntityTypes.Player:
                    data = new PlayerData();
                    break;
                case EntityTypes.Bullet:
                    data = new BulletData();
                    break;
                case EntityTypes.PowerUp:
                    data = new PowerUpData();
                    break;
                case EntityTypes.Asteroid:
                    data = new AsteroidData();
                    break;
                case EntityTypes.Ray:
                    data = new RayData();
                    break;
            }
            data.FromBytes(reader);
            return data;
        }

        public virtual void FromBytes(BinaryReader reader)
        {

        }

        public virtual void ToBytes(BinaryWriter writer)
        {
            writer.Write((byte)Type);
        }
    }

    public class PlayerData : EntityData
    {
        public string Name { get; set; }
        public byte Color { get; set; }
        public Vector2 Position { get; set; }
        public float Direction { get; set; }
        public float Speed { get; set; }
        public float TurretDirection { get; set; }
        public PlayerData()
            : base(EntityTypes.Player)
        { }

        public override void FromBytes(BinaryReader reader)
        {
            base.FromBytes(reader);
            Name = reader.ReadString();
            Color = reader.ReadByte();
            Position = new Vector2(reader.ReadSingle(), reader.ReadSingle());
            Direction = reader.ReadSingle();
            Speed = reader.ReadSingle();
            TurretDirection = reader.ReadSingle();
        }

        public override void ToBytes(BinaryWriter writer)
        {
            base.ToBytes(writer);
            writer.Write(Name);
            writer.Write(Color);
            writer.Write(Position.X);
            writer.Write(Position.Y);
            writer.Write(Direction);
            writer.Write(Speed);
            writer.Write(TurretDirection);
        }
    }

    public class BulletData : EntityData
    {
        public uint PlayerID { get; set; }
        public Vector2 Position { get; set; }
        public float Direction { get; set; }
        public float Speed { get; set; }

        public BulletData()
            : base(EntityTypes.Bullet)
        { }

        public override void FromBytes(BinaryReader reader)
        {
            base.FromBytes(reader);
            PlayerID = reader.ReadUInt32();
            Position = new Vector2(reader.ReadSingle(), reader.ReadSingle());
            Direction = reader.ReadSingle();
            Speed = reader.ReadSingle();
        }

        public override void ToBytes(BinaryWriter writer)
        {
            base.ToBytes(writer);
            writer.Write(PlayerID);
            writer.Write(Position.X);
            writer.Write(Position.Y);
            writer.Write(Direction);
            writer.Write(Speed);
        }
    }

    public class PowerUpData : EntityData
    {
        public PowerUpTypes PowerUpType { get; set; }
        public Vector2 Position { get; set; }

        public PowerUpData()
            : base(EntityTypes.PowerUp)
        { }

        public override void FromBytes(BinaryReader reader)
        {
            base.FromBytes(reader);
            PowerUpType = (PowerUpTypes)reader.ReadByte();
            Position = new Vector2(reader.ReadSingle(), reader.ReadSingle());
        }

        public override void ToBytes(BinaryWriter writer)
        {
            base.ToBytes(writer);
            writer.Write((byte)PowerUpType);
            writer.Write(Position.X);
            writer.Write(Position.Y);
        }
    }

    public class AsteroidData : EntityData
    {
        public byte ShapeID { get; set; }
        public Vector2 Position { get; set; }
        public float Direction { get; set; }
        public float RotationSpeed { get; set; }

        public AsteroidData()
            : base(EntityTypes.Asteroid)
        { }

        public override void FromBytes(BinaryReader reader)
        {
            base.FromBytes(reader);
            ShapeID = reader.ReadByte();
            Position = new Vector2(reader.ReadSingle(), reader.ReadSingle());
            Direction = reader.ReadSingle();
            RotationSpeed = reader.ReadSingle();
        }

        public override void ToBytes(BinaryWriter writer)
        {
            base.ToBytes(writer);
            writer.Write(ShapeID);
            writer.Write(Position.X);
            writer.Write(Position.Y);
            writer.Write(Direction);
            writer.Write(RotationSpeed);
        }
    }

    public class RayData : EntityData
    {
        public uint PlayerID { get; set; }
        public Vector2 Position { get; set; }
        public float Direction { get; set; }

        public RayData()
            : base(EntityTypes.Ray)
        { }

        public override void FromBytes(BinaryReader reader)
        {
            base.FromBytes(reader);
            PlayerID = reader.ReadUInt32();
            Position = new Vector2(reader.ReadSingle(), reader.ReadSingle());
            Direction = reader.ReadSingle();
        }

        public override void ToBytes(BinaryWriter writer)
        {
            base.ToBytes(writer);
            writer.Write(PlayerID);
            writer.Write(Position.X);
            writer.Write(Position.Y);
            writer.Write(Direction);
        }
    }
}
