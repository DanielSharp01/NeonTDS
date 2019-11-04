using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Numerics;

namespace NeonTDS
{
    public class Entity
    {
        protected EntityManager entityManager;
        public string ID { get; set; }
        public Vector2 Position { get; set; }
        public float Speed { get; set; }
        public float Direction { get; set; }
        [JsonIgnore]
        public Shape Shape { get; protected set; }
        [JsonIgnore]
        public float BoundingRadius { get; private set; }
        public Vector4 Color { get; set; }

        [JsonIgnore]
        public Matrix3x2 Transformation => Matrix3x2.CreateTranslation(-Shape.Origin) *
            Matrix3x2.CreateRotation(Direction) *
            Matrix3x2.CreateTranslation(Position);

        public event Action<Entity> OnCollisionWith;

        public void ClearEvents()
        {
            OnCollisionWith = null;
        }

        public Entity(EntityManager entityManager, Shape shape)
        {
            this.entityManager = entityManager;
            ID = Guid.NewGuid().ToString();
            Shape = shape;
            CalculateBoundingRadius();            
        }

        public virtual void PostSerialize(EntityManager entityManager)
        {
            this.entityManager = entityManager;
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

        }

        public virtual void OnDestroy()
        {

        }

        public void CalculateBoundingRadius()
        {
            BoundingRadius = (Shape.Points.First() - Shape.Origin).LengthSquared();
            foreach (Vector2 point in Shape.Points)
            {
                float distance = (point - Shape.Origin).LengthSquared();
                if (distance > BoundingRadius) BoundingRadius = distance;
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