using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Numerics;

namespace NeonTDS
{
    public class Entity
    {
        public static uint NextID = 0;
        protected EntityManager entityManager;
        public uint ID { get; set; }
        public Vector2 Position { get; set; }
        public float Speed { get; set; }
        public float Direction { get; set; }
        public Shape Shape { get; /*protected*/ set; }
        public Vector4 Color { get; set; }

        public uint? CreationTick { get; set; } = null;
        public uint? DestructionTick { get; set; } = null;

        public Matrix3x2 Transformation => Matrix3x2.CreateTranslation(-Shape.Origin) *
            Matrix3x2.CreateRotation(Direction) *
            Matrix3x2.CreateTranslation(Position);

        public virtual bool IsRenderOnly => false;

        public event Action<Entity> OnCollisionWith;

        public void ClearEvents()
        {
            OnCollisionWith = null;
        }

        public Entity(EntityManager entityManager, Shape shape)
        {
            this.entityManager = entityManager;
            
            // Only allow half the IDs for each side make them rollover
            ID = NextID++ + (entityManager.ServerSide ? 0 : uint.MaxValue / 2);
            NextID %= uint.MaxValue;

            Shape = shape;
        }

        public virtual void UpdateEntity(Entity data)
        {
            Position = data.Position;
            Speed = data.Speed;
            Direction = data.Direction;
            Color = data.Color;
        }

        public virtual void OnCreate()
        {
            if (entityManager.ServerSide)
            {
                CreationTick = entityManager.Clock;
            }
        }

        public virtual void OnDestroy()
        {
            if (entityManager.ServerSide)
            {
                DestructionTick = entityManager.Clock;
            }
        }

        public virtual void Update(float elapsedTimeSeconds)
        {
            Position += GameMath.Vector2FromAngle(Direction) * Speed * elapsedTimeSeconds;
        }

        public virtual void CollidesWith(Entity other)
        {
            OnCollisionWith?.Invoke(other);
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is Entity entity) return entity.ID == ID;
            else return false;
        }
    }
}