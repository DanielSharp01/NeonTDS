using System;
using System.Linq;
using System.Numerics;

namespace NeonTDS
{
    public class Entity
    {
        protected EntityManager entityManager;
        public Guid ID { get; private set; }
        public Vector2 Position { get; set; }
        public float Speed { get; set; }
        public float Direction { get; set; }
        public Shape Shape { get; }
        public float BoundingRadius { get; private set; }
        public Vector4 Color { get; set; }

        public Matrix3x2 Transformation => Matrix3x2.CreateTranslation(-Shape.Origin) *
            Matrix3x2.CreateRotation(Direction) *
            Matrix3x2.CreateTranslation(Position);

        public event Action<Entity> OnCollisionWith;

        public void CleaEvents()
        {
            OnCollisionWith = null;
        }

        public Entity(EntityManager entityManager, Shape shape)
        {
            this.entityManager = entityManager;
            // TODO: This obviously comes from outside the entity
            ID = Guid.NewGuid();

            BoundingRadius = (shape.Points.First() - shape.Origin).LengthSquared();
            foreach (Vector2 point in shape.Points)
            {
                float distance = (point - shape.Origin).LengthSquared();
                if (distance > BoundingRadius) BoundingRadius = distance;
            }


            Shape = shape;
        }

        public virtual void OnCreate()
        {

        }

        public virtual void OnDestroy()
        {

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